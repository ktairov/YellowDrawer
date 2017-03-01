using System;
using System.IO;
using YellowDrawer.Storage.Common;
using System.Linq;
using MongoDB.Driver.GridFS;

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
    }
}
