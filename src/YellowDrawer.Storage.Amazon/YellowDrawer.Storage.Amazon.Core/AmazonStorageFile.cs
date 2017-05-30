using Amazon.S3;
using System;
using System.IO;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.AmazonS3
{
    public class AmazonStorageFile : IStorageFile
    {
        private readonly S3FileInfo _fileInfo;
        public AmazonStorageFile(S3FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        #region Implementation of IStorageFile
        public string GetPath()
        {
            return _fileInfo.DirectoryName;
        }

        public string GetFullPath()
        {
            return _fileInfo.FullName;
        }

        public string GetName()
        {
            return _fileInfo.Name;
        }

        public long GetSize()
        {
            return _fileInfo.Length;
        }

        public DateTime GetLastUpdated()
        {
            return _fileInfo.LastWriteTime;
        }

        public string GetFileType()
        {
            return _fileInfo.Extension;
        }

        public Stream OpenRead()
        {
            return _fileInfo.OpenRead();
        }

        public Stream OpenWrite()
        {
            return _fileInfo.OpenWrite();
        }

        public Stream OpenCryptoRead(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            return encryptionProvider.Decrypt(_fileInfo.OpenRead(), iv);
        }

        public Stream OpenCryptoWrite(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            return encryptionProvider.Encrypt(_fileInfo.OpenWrite(), iv);
        }
        #endregion
    }
}
