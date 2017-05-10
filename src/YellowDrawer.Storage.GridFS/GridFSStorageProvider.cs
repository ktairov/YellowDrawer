using System;
using System.Collections.Generic;
using YellowDrawer.Storage.Common;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

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


        //Async
        public async Task<IStorageFile> CreateFileAsync(string path, byte[] arr = null)
        {
            if (arr == null)
                arr = new byte[0];
            var id = await _bucket.UploadFromBytesAsync(path, arr);
            var info = _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", id)).Result.First();
            return new GridFSFileStorage(info, _bucket);
        }

        public async void CreateFolderAsync(string path)
        {
            var arr = new byte[0];
            path = path.TrimEnd('\\') + '\\';
            var id = await _bucket.UploadFromBytesAsync(path, arr);
        }

        public async void DeleteFileAsync(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                await _bucket.DeleteAsync(fileResult.Id);
        }

        public async void DeleteFolderAsync(string path)
        {
            var files = _bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            path = path.TrimEnd('\\') + '\\';
            var fileResult = files.FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                await _bucket.DeleteAsync(fileResult.Id);
        }

        public async Task<IStorageFile> GetFileAsync(string path)
        {
            var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty);
            var fileResult = files.ToList().FirstOrDefault(x => x.Filename == path);
            if (fileResult == null)
                return null;
            return new GridFSFileStorage(fileResult, _bucket);
        }

        public async Task<string> GetPublicUrlAsync(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsFileExistsAsync(string path)
        {
            var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty);
            var fileResult = files.ToList().FirstOrDefault(x => x.Filename == path);
            return fileResult != null;
        }

        public async Task<bool> IsFolderExitsAsync(string path)
        {
            var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty);
            path = path.TrimEnd('\\') + '\\';
            var fileResult = files.ToList().FirstOrDefault(x => x.Filename == path);
            return fileResult != null;
        }

        public async Task<IEnumerable<IStorageFile>> ListFilesAsync(string path)
        {
            var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty);
            return files.ToList().Where(x => x.Filename.StartsWith(path) && !x.Filename.EndsWith("\\")).Select(x => new GridFSFileStorage(x, _bucket));
        }

        public async Task<IEnumerable<IStorageFolder>> ListFoldersAsync(string path)
        {
            var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty);
            return files.ToList().Where(x => x.Filename.StartsWith(path) && x.Filename.EndsWith("\\")).Select(x => new GridFSFolderStorage(_bucket, x));
        }

        public async void RenameFileAsync(string path, string newPath)
        {
            var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty);
            var fileResult = files.ToList().FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                await _bucket.RenameAsync(fileResult.Id, newPath);
        }

        public async void RenameFolderAsync(string path, string newPath)
        {
            var files = await _bucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty);
            var fileResult = files.ToList().FirstOrDefault(x => x.Filename == path);
            if (fileResult != null)
                await _bucket.RenameAsync(fileResult.Id, newPath);
        }
    }
}
