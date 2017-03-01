using Microsoft.WindowsAzure.StorageClient;
using System;
using System.IO;
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

            _blob.FetchAttributes();
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
            return _blob.Properties.LastModifiedUtc == null ? DateTime.MinValue : _blob.Properties.LastModifiedUtc;
        }

        public string GetFileType()
        {
            return Path.GetExtension(GetPath());
        }

        public Stream OpenRead()
        {
            return _blob.OpenRead();
        }

        public Stream OpenWrite()
        {
            return _blob.OpenWrite();
        }

        public Stream CreateFile()
        {
            _blob.DeleteIfExists();
            _blob = _blob.Container.GetBlockBlobReference(_blob.Uri.ToString());
            _blob.OpenWrite().Dispose();

            return OpenWrite();
        }

        public string GetFullPath()
        {
            throw new NotImplementedException();
        }
    }
}