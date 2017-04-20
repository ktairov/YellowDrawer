using System;
using System.IO;
using System.Linq;
using Amazon.S3.IO;
using YellowDrawer.Common;

namespace YellowDrawer.AmazonS3
{
    public class AmazonStorageFolder : IStorageFolder
    {
        private readonly S3DirectoryInfo _directoryInfo;
        private readonly string _path;

        public AmazonStorageFolder(string path, S3DirectoryInfo directoryInfo)
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
                return new AmazonStorageFolder(Path.GetDirectoryName(_path), _directoryInfo.Parent);
            }
            throw new InvalidOperationException("Directory " + _directoryInfo.Name +
                                                " does not have a parent directory");
        }

        #endregion

        private static long GetDirectorySize(S3DirectoryInfo directoryInfo)
        {
            var fileInfos = directoryInfo.GetFiles();
            var size = fileInfos.Sum(fileInfo => fileInfo.Length);
            var directoryInfos = directoryInfo.GetDirectories();
            size += directoryInfos.Sum(dInfo => GetDirectorySize(dInfo));
            return size;
        }
    }
}
