using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.S3;
using Amazon.S3.IO;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.AmazonS3
{
    public class AmazonStorageProvider : IStorageProvider
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly string _baseUrl;
        private readonly string _bucketName;

        public AmazonStorageProvider(IAmazonS3 amazonS3, string baseUrl, string bucketName)
        {
            _amazonS3 = amazonS3;
            _baseUrl = baseUrl;
            _bucketName = bucketName;
        }

        private static string Fix(string path)
        {
            return string.IsNullOrEmpty(path)
                ? string.Empty
                : Path.DirectorySeparatorChar != '/'
                    ? path.Replace('/', Path.DirectorySeparatorChar)
                    : path;
        }

        #region Implementation of IStorageProvider

        public string GetPublicUrl(string path)
        {
            return string.Format("https://{0}.{1}/{2}", _bucketName, _baseUrl, path.Replace(Path.DirectorySeparatorChar, '/'));
        }

        public IStorageFile GetFile(string path)
        {
            if (!IsFileExists(path))
            {
                throw new InvalidOperationException("File " + path + " does not exist");
            }
            return new AmazonStorageFile(new S3FileInfo(_amazonS3, _bucketName, path));
        }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            if (!IsFolderExits(path))
            {
                throw new InvalidOperationException("Directory " + path + " does not exist");
            }

            return new S3DirectoryInfo(_amazonS3, _bucketName, path)
                .GetFiles("*.*", SearchOption.AllDirectories)
                .Select<S3FileInfo, IStorageFile>(fi => new AmazonStorageFile(fi))
                .ToList();
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            if (!IsFolderExits(path))
            {
                try
                {
                    CreateFolder(path);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format("The folder could not be created at path: {0}. {1}", path, ex));
                }
            }
            return new S3DirectoryInfo(_amazonS3, _bucketName, path)
                .GetDirectories("*", SearchOption.AllDirectories)
                .Select<S3DirectoryInfo, IStorageFolder>(di => new AmazonStorageFolder(Path.Combine(Fix(path), di.Name), di))
                .ToList();
        }

        public void CreateFolder(string path)
        {
            if (IsFolderExits(path))
            {
                throw new InvalidOperationException("Directory " + path + " already exists");
            }

            var di = new S3DirectoryInfo(_amazonS3, _bucketName, path);
            di.Create();
        }

        public void DeleteFolder(string path)
        {
            if (!IsFolderExits(path))
            {
                throw new InvalidOperationException("Directory " + path + " does not exist");
            }

            var folderInfo = new S3DirectoryInfo(_amazonS3, _bucketName, path);
            folderInfo.Delete(true);
        }

        public void RenameFolder(string path, string newPath)
        {
            if (!IsFolderExits(path))
            {
                throw new InvalidOperationException("Directory " + path + "does not exist");
            }

            if (IsFolderExits(newPath))
            {
                throw new InvalidOperationException("Directory " + newPath + " already exists");
            }

            var di = new S3DirectoryInfo(_amazonS3, _bucketName, path);
            var newDi = new S3DirectoryInfo(_amazonS3, _bucketName, newPath);
            di.MoveTo(newDi);
        }

        public IStorageFile CreateFile(string path, byte[] arr = null)
        {
            if (IsFileExists(path))
            {
                throw new InvalidOperationException("File " + path + " already exists");
            }

            var fileInfo = new S3FileInfo(_amazonS3, _bucketName, path);
            using (var stream = fileInfo.Create())
            {
                if (arr == null)
                    stream.Write(new byte[0], 0, 0);
                else
                    stream.Write(arr, 0, arr.Length);
            }

            return new AmazonStorageFile(fileInfo);
        }

        public bool IsFileExists(string path)
        {
            var fileInfo = new S3FileInfo(_amazonS3, _bucketName, path);
            return fileInfo.Exists;
        }

        public bool IsFolderExits(string path)
        {
            var folderInfo = new S3DirectoryInfo(_amazonS3, _bucketName, path);
            return folderInfo.Exists;
        }

        public void DeleteFile(string path)
        {
            if (!IsFileExists(path))
            {
                throw new InvalidOperationException("File " + path + " does not exist");
            }

            var fileInfo = new S3FileInfo(_amazonS3, _bucketName, path);
            fileInfo.Delete();
        }

        public void RenameFile(string path, string newPath)
        {
            if (!IsFileExists(path))
            {
                throw new InvalidOperationException("File " + path + " does not exist");
            }

            if (IsFileExists(newPath))
            {
                throw new InvalidOperationException("File " + newPath + " already exists");
            }

            var fileInfo = new S3FileInfo(_amazonS3, _bucketName, path);
            var newFileInfo = new S3FileInfo(_amazonS3, _bucketName, newPath);
            fileInfo.MoveTo(newFileInfo);
        }

        #endregion
    }
}
