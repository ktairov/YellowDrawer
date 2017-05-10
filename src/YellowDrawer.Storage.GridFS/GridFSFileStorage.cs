using System;
using System.IO;
using YellowDrawer.Storage.Common;
using System.Linq;
using MongoDB.Driver.GridFS;
using System.Threading.Tasks;

namespace YellowDrawer.Storage.GridFS
{
    public class GridFSFileStorage : IStorageFile
    {
        GridFSFileInfo _fileInfo;
        IGridFSBucket _bucket;

        public GridFSFileStorage(GridFSFileInfo fileInfo, IGridFSBucket bucket)
        {
            _bucket = bucket;
            _fileInfo = fileInfo;
        }

        public string GetFileType()
        {
            return _fileInfo.Filename.Split('.').ToList().Last();
        }

        public string GetFullPath()
        {
            return _fileInfo.Filename;
        }

        public DateTime GetLastUpdated()
        {
            return _fileInfo.UploadDateTime;
        }

        public string GetName()
        {
            return _fileInfo.Filename;
        }

        public string GetPath()
        {
            return _fileInfo.Filename;
        }

        public long GetSize()
        {
            return _fileInfo.Length;
        }

        public Stream OpenRead()
        {
            return _bucket.OpenDownloadStream(_fileInfo.Id);
        }

        public Stream OpenWrite()
        {
            return _bucket.OpenUploadStream(_fileInfo.Filename);
        }

        public async Task<Stream> OpenReadAsync()
        {
            return await _bucket.OpenDownloadStreamAsync(_fileInfo.Id);
        }

        public async Task<Stream> OpenWriteAsync()
        {
            return await _bucket.OpenUploadStreamAsync(_fileInfo.Filename);
        }

        public Stream OpenCryptoRead(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            return encryptionProvider.Decrypt(_bucket.OpenDownloadStream(_fileInfo.Id), iv);
        }

        public Stream OpenCryptoWrite(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            return encryptionProvider.Encrypt(_bucket.OpenUploadStream(_fileInfo.Filename), iv);
        }
    }
}
