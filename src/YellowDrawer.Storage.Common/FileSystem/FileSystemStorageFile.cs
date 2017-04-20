using System;
using System.IO;

namespace YellowDrawer.Storage.Common
{
    public class FileSystemStorageFile : IStorageFile
    {
        private readonly FileInfo _fileInfo;
        private readonly string _path;

        public FileSystemStorageFile(string path, FileInfo fileInfo)
        {
            _path = path;
            _fileInfo = fileInfo;
        }

        #region Implementation of IStorageFile
        public string GetFullPath()
        {
            return Path.Combine(_path, _fileInfo.Name);
        }

        public string GetPath()
        {
            return _path;
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
            return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read);
        }

        public Stream OpenWrite()
        {
            return new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
        }
        #endregion
    }
}
