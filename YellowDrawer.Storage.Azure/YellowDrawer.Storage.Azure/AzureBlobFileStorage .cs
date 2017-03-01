using System;
using System.Globalization;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using YellowDrawer.Common;

namespace YellowDrawer.Azure
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
            return _blob.Properties.LastModified == null ? DateTime.MinValue : _blob.Properties.LastModified.Value.UtcDateTime;
        }

        public string GetFileType()
        {
            return Path.GetExtension(GetPath());
        }

        public string GetSharedAccessPath(DateTimeOffset? expiration = null, SasPermissionFlags permissions = SasPermissionFlags.Read)
        {
            var sasToken = _blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = expiration ?? _azureFileSystem.DefaultSharedAccessExpiration,
                Permissions = GetSharedAccessBlobPermissions(permissions)
            });

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", _blob.Uri, sasToken);
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

        private static SharedAccessBlobPermissions GetSharedAccessBlobPermissions(SasPermissionFlags flags)
        {
            return (SharedAccessBlobPermissions)flags;
        }

        public string GetFullPath()
        {
            throw new NotImplementedException();
        }
    }
}