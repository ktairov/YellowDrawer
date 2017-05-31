using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Azure
{
    public class AzureBlobFileStorage : IStorageFile
    {
        private CloudBlockBlob _blob;
        private readonly AzureBlobStorageProvider _azureFileSystem;
        private bool _hasFetchedAttributes = false;

        public AzureBlobFileStorage(CloudBlockBlob blob, AzureBlobStorageProvider azureFileSystem)
        {
            _blob = blob;
            _azureFileSystem = azureFileSystem;
        }

        private void EnsureAttributes()
        {
            if (_hasFetchedAttributes)
                return;

            _blob.FetchAttributesAsync();
            _hasFetchedAttributes = true;
        }

        public string GetPath()
        {
            return _azureFileSystem.Combine(_blob.Container.Name, _blob.Name);
        }

        public string GetName()
        {
            return Path.GetFileName(GetPath());
        }

        public long GetSize()
        {
            EnsureAttributes();
            return _blob.Properties.Length;
        }

        public DateTime GetLastUpdated()
        {
            EnsureAttributes();
            return _blob.Properties.LastModified == null ? DateTime.MinValue : _blob.Properties.LastModified.Value.UtcDateTime;
        }

        public string GetFileType()
        {
            return Path.GetExtension(GetPath());
        }

        public Stream OpenRead()
        {
            return _blob.OpenReadAsync().Result;
        }

        public Stream OpenWrite()
        {
            return _blob.OpenWriteAsync().Result;
        }

        public Stream OpenCryptoRead(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            return encryptionProvider.Decrypt(_blob.OpenReadAsync().Result, iv);
        }

        public Stream OpenCryptoWrite(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            return encryptionProvider.Encrypt(_blob.OpenWriteAsync().Result, iv);
        }

        public Stream CreateFile()
        {
            _blob.DeleteIfExistsAsync();
            _blob = _blob.Container.GetBlockBlobReference(_blob.Uri.ToString());
            _blob.OpenWriteAsync().Result.Dispose();
            return OpenWrite();
        }

        public string GetFullPath()
        {
            throw new NotImplementedException();
        }
    }
}