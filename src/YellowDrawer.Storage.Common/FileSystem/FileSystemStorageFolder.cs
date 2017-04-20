using System;
using System.IO;
using System.Linq;

namespace YellowDrawer.Storage.Common
{
    public class FileSystemStorageFolder : IStorageFolder
    {
        private readonly DirectoryInfo _directoryInfo;
        private readonly string _path;

        public FileSystemStorageFolder(string path, DirectoryInfo directoryInfo)
        {
            _path = path;
            _directoryInfo = directoryInfo;
        }

        #region Implementation of IStorageFolder

        public string GetPath()
        {
            return _path;
        }

        public string GetName()
        {
            return _directoryInfo.Name;
        }

        public DateTime GetLastUpdated()
        {
            return _directoryInfo.LastWriteTime;
        }

        public long GetSize()
        {
            return GetDirectorySize(_directoryInfo);
        }

        public IStorageFolder GetParent()
        {
            if (_directoryInfo.Parent != null)
            {
                return new FileSystemStorageFolder(Path.GetDirectoryName(_path), _directoryInfo.Parent);
            }
            throw new InvalidOperationException("Directory " + _directoryInfo.Name +
                                                " does not have a parent directory");
        }

        #endregion

        private static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            var fileInfos = directoryInfo.GetFiles();
            var size = fileInfos.Where(fileInfo => !FileSystemStorageProvider.IsHidden(fileInfo)).Sum(fileInfo => fileInfo.Length);
            var directoryInfos = directoryInfo.GetDirectories();
            size += directoryInfos.Where(dInfo => !FileSystemStorageProvider.IsHidden(dInfo)).Sum(dInfo => GetDirectorySize(dInfo));
            return size;
        }
    }
}
