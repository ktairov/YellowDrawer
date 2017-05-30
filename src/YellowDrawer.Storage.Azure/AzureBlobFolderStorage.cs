using Microsoft.WindowsAzure.StorageClient;
using System;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Azure
{
    public class AzureBlobFolderStorage : IStorageFolder
    {
        private readonly CloudBlobDirectory _blob;
        private readonly AzureBlobStorageProvider _azureFileSystem;

        public AzureBlobFolderStorage(CloudBlobDirectory blob, AzureBlobStorageProvider azureFileSystem)
        {
            _blob = blob;
            _azureFileSystem = azureFileSystem;
        }

        public string GetName()
        {
            return _blob.Container.Name.Trim('/');
        }

        public string GetPath()
        {
            return _blob.Uri.ToString().Replace(_blob.Container.Uri.ToString(), _blob.Container.Name).Trim('/');
        }

        public long GetSize()
        {
            return GetDirectorySize(_blob);
        }

        public DateTime GetLastUpdated()
        {
            return DateTime.MinValue;
        }

        public IStorageFolder GetParent()
        {
            if (_blob.Parent != null)
            {
                return new AzureBlobFolderStorage(_blob.Parent, _azureFileSystem);
            }
            throw new ArgumentException("Directory " + _blob.Uri + " does not have a parent directory");
        }

        private static long GetDirectorySize(CloudBlobDirectory directoryBlob)
        {
            long size = 0;

            foreach (var blobItem in directoryBlob.ListBlobs())
            {
                if (blobItem is CloudBlockBlob)
                    size += ((CloudBlockBlob)blobItem).Properties.Length;

                if (blobItem is CloudBlobDirectory)
                    size += GetDirectorySize((CloudBlobDirectory)blobItem);
            }

            return size;
        }

    }
}