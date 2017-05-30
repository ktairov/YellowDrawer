using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Azure
{
    public class AzureBlobStorageProvider : IStorageProvider
    {
        private object _lock = new object();
        public CloudBlobClient BlobClient { get; private set; }
        public IList<CloudBlobContainer> Containers { get; private set; }
        public Func<string, CloudBlobContainer> ContainerFactory;
        private CloudStorageAccount _storageAccount;

        public AzureBlobStorageProvider(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
            BlobClient = _storageAccount.CreateCloudBlobClient();
            Containers = new List<CloudBlobContainer>();
            ContainerFactory = CreateContainer;
        }

        private CloudBlobContainer EnsurePathIsRelativeAndEnsureContainer(ref string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            path = path.Replace('\\', '/').ToLower().TrimStart('/');
            if (path.StartsWith("http://") || path.StartsWith("https://"))
                throw new ArgumentException("Path must be relative");

            var containerName = path.Split('/').First();
            CloudBlobContainer container;
            lock (_lock)
            {
                container = Containers.SingleOrDefault(c => c.Name == containerName);
                if (container == null)
                {
                    container = BlobClient.GetContainerReference(containerName);
                    if (!container.ExistsAsync().Result)
                    {
                        container = ContainerFactory(containerName);
                    }
                    Containers.Add(container);
                }
            }
            path = string.Join("/", path.Split('/').Skip(1).ToArray());
            return container;
        }

        public string Combine(string path1, string path2)
        {
            if (path1 == null)
            {
                throw new ArgumentNullException("path1");
            }
            if (path2 == null)
            {
                throw new ArgumentNullException("path2");
            }
            return (path1.TrimEnd('/') + '/' + path2.TrimStart('/'));
        }

        public string GetPublicUrl(string path)
        {
            return path;
        }

        public IStorageFile GetFile(string path)
        {
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            container.EnsureBlobExists(path);
            return new AzureBlobFileStorage(container.GetBlockBlobReference(path), this);
        }

        public bool FileExists(string path)
        {
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            return container.BlobExists(path);
        }

        public DateTimeOffset? DefaultSharedAccessExpiration { get; set; }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            path = path ?? string.Empty;
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            string prefix = Combine(container.Name, path);
            prefix = prefix.TrimEnd('/') + "/";
            var result = BlobClient.ListBlobsSegmentedAsync(prefix, null).Result.Results
                .OfType<CloudBlockBlob>()
                .Where(b => !b.Name.EndsWith("/"))
                .Select(blobItem => new AzureBlobFileStorage(blobItem, this))
                .ToArray();

            return result;
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            path = path ?? string.Empty;
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            if (path == string.Empty)
            {
                return container.ListBlobsSegmentedAsync(null).Result.Results
                    .OfType<CloudBlobDirectory>()
                    .Select<CloudBlobDirectory, IStorageFolder>(d => new AzureBlobFolderStorage(d, this))
                    .ToList();
            }

            return container.GetDirectoryReference(path)
                .ListBlobsSegmentedAsync(null).Result.Results
                .OfType<CloudBlobDirectory>()
                .Select<CloudBlobDirectory, IStorageFolder>(d => new AzureBlobFolderStorage(d, this))
                .ToList();
        }

        public void CreateFolder(string path)
        {
            string fullPath = path;
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            container.EnsureDirectoryDoesNotExist(path);
            CreateFile(fullPath.Trim('/') + "/");
        }

        public void DeleteFolder(string path)
        {
            var fullPath = path;
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            if (path == "")
            {
                Containers.Remove(container);
                container.DeleteAsync();
                return;
            }
            container.EnsureDirectoryExists(path);
            foreach (var blob in container.GetDirectoryReference(path).ListBlobsSegmentedAsync(null).Result.Results)
            {
                if (blob is CloudBlockBlob)
                    ((CloudBlockBlob)blob).DeleteAsync();

                if (blob is CloudBlobDirectory)
                    DeleteFolder(new AzureBlobFolderStorage((CloudBlobDirectory)blob, this).GetPath());
            }
        }

        public void RenameFolder(string path, string newPath)
        {
            var fullNewPath = newPath;

            EnsurePathIsRelativeAndEnsureContainer(ref path);
            var container = EnsurePathIsRelativeAndEnsureContainer(ref newPath);

            if (path == "")
                throw new ArgumentException("Renaming root folders represented by azure containers is not currently supported", path);

            path = path.TrimEnd('/') + '/';
            fullNewPath = fullNewPath.TrimEnd('/') + '/';

            foreach (var blob in container.GetDirectoryReference(path).ListBlobsSegmentedAsync(null).Result.Results)
            {
                if (blob is CloudBlockBlob)
                {
                    var azureBlob = new AzureBlobFileStorage((CloudBlockBlob)blob, this);
                    RenameFile(azureBlob.GetPath(), Combine(fullNewPath, azureBlob.GetName()));
                }
                if (blob is CloudBlobDirectory)
                {
                    var azureFolder = new AzureBlobFolderStorage((CloudBlobDirectory)blob, this);
                    RenameFolder(azureFolder.GetPath(), Path.Combine(fullNewPath, azureFolder.GetName()));
                }
            }
        }

        public void DeleteFile(string path)
        {
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            container.EnsureBlobExists(path);
            var blob = container.GetBlockBlobReference(path);
            blob.DeleteAsync();
        }

        public void RenameFile(string path, string newPath)
        {
            EnsurePathIsRelativeAndEnsureContainer(ref path);
            var container = EnsurePathIsRelativeAndEnsureContainer(ref newPath);

            container.EnsureBlobExists(path);
            container.EnsureBlobDoesNotExist(newPath);

            var blob = container.GetBlockBlobReference(path);
            var newBlob = container.GetBlockBlobReference(newPath);
            newBlob.StartCopyAsync(blob).Wait();
            blob.DeleteAsync();
        }

        public IStorageFile CreateFile(string path, byte[] arr = null)
        {
            if (arr == null)
                arr = new byte[0];

            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            if (container.BlobExists(path))
            {
                throw new ArgumentException("File " + path + " already exists");
            }
            var blob = container.GetBlockBlobReference(path);
            blob.UploadFromByteArrayAsync(arr, 0, arr.Length).Wait();
            return new AzureBlobFileStorage(blob, this);
        }

        public bool IsFileExists(string path)
        {
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            return container.BlobExists(path);
        }

        public bool IsFolderExits(string path)
        {
            var container = EnsurePathIsRelativeAndEnsureContainer(ref path);
            return container.DirectoryExists(path);
        }

        public bool TrySaveStream(string path, Stream inputStream)
        {
            try
            {
                SaveStream(path, inputStream);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void SaveStream(string path, Stream inputStream)
        {
            var file = CreateFile(path);
            using (var outputStream = file.OpenWrite())
            {
                var buffer = new byte[8192];
                for (;;)
                {
                    var length = inputStream.Read(buffer, 0, buffer.Length);
                    if (length <= 0)
                        break;
                    outputStream.Write(buffer, 0, length);
                }
            }
        }

        private CloudBlobContainer CreateContainer(string name)
        {
            var container = BlobClient.GetContainerReference(name);
            container.CreateIfNotExistsAsync();
            return container;
        }
    }
}