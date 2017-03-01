using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Linq;
using YellowDrawer.Common;

namespace YellowDrawer.GridFS
{
    public class GridFSFolderStorage : IStorageFolder
    {
        IGridFSBucket _bucket;
        GridFSFileInfo _fileInfo;

        public GridFSFolderStorage(IGridFSBucket bucket, GridFSFileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            _bucket = bucket;
        }

        public DateTime GetLastUpdated()
        {
            return _fileInfo.UploadDateTime;
        }

        public string GetName()
        {
            return _fileInfo.Filename.TrimEnd('\\').Split('\\').Last();
        }

        public IStorageFolder GetParent()
        {
            var pathParts = _fileInfo.Filename.TrimEnd('\\').Split('\\');
            var result = string.Empty;
            for (var i = 0; i < pathParts.Length - 1; i++)
                result += pathParts[i] + "\\";
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            var fileResult = files.FirstOrDefault(x => x.Filename == result);
            if (fileResult == null)
            {
                var arr = new byte[0];
                result = result.TrimEnd('\\') + '\\';
                var id = _bucket.UploadFromBytes(result, arr);
                fileResult = _bucket.Find(Builders<GridFSFileInfo>.Filter.Eq("_id", id)).First();
            }
            return new GridFSFolderStorage(_bucket, fileResult);
        }

        public string GetPath()
        {
            return _fileInfo.Filename;
        }

        public long GetSize()
        {
            return 0;
        }
    }
}
