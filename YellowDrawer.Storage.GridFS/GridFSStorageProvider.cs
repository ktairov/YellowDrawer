using System;
using System.Collections.Generic;
using YellowDrawer.Storage.Common;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using System.Linq;

namespace YellowDrawer.Storage.GridFS
{
    public class GridFSStorageProvider : IStorageProvider
    {
        IGridFSBucket _bucket;

        public GridFSStorageProvider(IGridFSBucket bucket)
        {
            _bucket = bucket;
        }

        public IStorageFile CreateFile(string path, byte[] arr = null)
        {
            if (arr == null)
                arr = new byte[0];
            var id = _bucket.UploadFromBytes(path, arr);
            var info = _bucket.Find(Builders<GridFSFileInfo>.Filter.Eq("_id", id)).First();
            return new GridFSFileStorage(info, _bucket);
        }

        public void CreateFolder(string path)
        {
            var arr = new byte[0];
            path = path.TrimEnd('\\') + '\\';
            var id = _bucket.UploadFromBytes(path, arr);
        }

        public void DeleteFile(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                _bucket.Delete(fileResult.Id);
        }

        public void DeleteFolder(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            path = path.TrimEnd('\\') + '\\';
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                _bucket.Delete(fileResult.Id);
        }

        public IStorageFile GetFile(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            if (fileResult == null)
                return null;
            return new GridFSFileStorage(fileResult, _bucket);
        }

        public string GetPublicUrl(string path)
        {
            throw new NotImplementedException();
        }

        public bool IsFileExists(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            return fileResult != null;
        }

        public bool IsFolderExits(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            path = path.TrimEnd('\\') + '\\';
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            return fileResult != null;
        }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList().Where(x=>x.Filename.StartsWith(path) && !x.Filename.EndsWith("\\"));
            return files.Select(x => new GridFSFileStorage(x, _bucket));
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList().Where(x => x.Filename.StartsWith(path) && x.Filename.EndsWith("\\"));
            return files.Select(x => new GridFSFolderStorage(_bucket, x));
        }

        public void RenameFile(string path, string newPath)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                _bucket.Rename(fileResult.Id, newPath);
        }

        public void RenameFolder(string path, string newPath)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                _bucket.Rename(fileResult.Id, newPath);
        }

        public bool TryCreateFolder(string path)
        {
            var arr = new byte[0];
            path = path.TrimEnd('\\') + '\\';
            var id = _bucket.UploadFromBytes(path, arr);
            return true;
        }
    }
}
