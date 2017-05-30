using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YellowDrawer.Storage.Common
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private readonly string _baseUrl;
        private readonly string _storagePath;

        public FileSystemStorageProvider(string basePath)
        {
            _storagePath = basePath;
            _baseUrl = basePath;

            _storagePath = basePath;
            if (!_baseUrl.EndsWith("/"))
                _baseUrl = _baseUrl + '/';
        }

        private string Map(string path)
        {
            return string.IsNullOrEmpty(path) ? Path.Combine(_baseUrl, _storagePath) : Path.Combine(_baseUrl, _storagePath + path);
        }

        private static string Fix(string path)
        {
            return string.IsNullOrEmpty(path)
                ? string.Empty
                : Path.DirectorySeparatorChar != '/'
                    ? path.Replace('/', Path.DirectorySeparatorChar)
                    : path;
        }

        #region Implementation of IStorageProvider

        public string GetPublicUrl(string path)
        {
            return _baseUrl + path.Replace(Path.DirectorySeparatorChar, '/');
        }

        public IStorageFile GetFile(string path)
        {
            if (!File.Exists(Map(path)))
            {
                throw new InvalidOperationException("File " + path + " does not exist");
            }
            return new FileSystemStorageFile(Fix(path), new FileInfo(Map(path)));
        }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            if (!Directory.Exists(Map(path)))
            {
                throw new InvalidOperationException("Directory " + path + " does not exist");
            }

            return new DirectoryInfo(Map(path))
                .GetFiles()
                .Where(fi => !IsHidden(fi))
                .Select<FileInfo, IStorageFile>(fi => new FileSystemStorageFile(Path.Combine(Fix(path), fi.Name), fi))
                .ToList();
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            if (!Directory.Exists(Map(path)))
            {
                try
                {
                    Directory.CreateDirectory(Map(path));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        string.Format("The folder could not be created at path: {0}. {1}", path, ex));
                }
            }

            return new DirectoryInfo(Map(path))
                .GetDirectories()
                .Where(di => !IsHidden(di))
                .Select<DirectoryInfo, IStorageFolder>(
                    di => new FileSystemStorageFolder(Path.Combine(Fix(path), di.Name), di))
                .ToList();
        }

        public void CreateFolder(string path)
        {
            if (Directory.Exists(Map(path)))
            {
                throw new InvalidOperationException("Directory " + path + " already exists");
            }

            Directory.CreateDirectory(Map(path));
        }

        public void DeleteFolder(string path)
        {
            if (!Directory.Exists(Map(path)))
            {
                throw new InvalidOperationException("Directory " + path + " does not exist");
            }

            Directory.Delete(Map(path), true);
        }

        public void RenameFolder(string path, string newPath)
        {
            if (!Directory.Exists(Map(path)))
            {
                throw new InvalidOperationException("Directory " + path + "does not exist");
            }

            if (Directory.Exists(Map(newPath)))
            {
                throw new InvalidOperationException("Directory " + newPath + " already exists");
            }

            Directory.Move(Map(path), Map(newPath));
        }

        public IStorageFile CreateFile(string path, byte[] arr = null)
        {
            if (arr == null)
                arr = new byte[0];

            if (File.Exists(Map(path)))
            {
                throw new InvalidOperationException("File " + path + " already exists");
            }

            var fileInfo = new FileInfo(Map(path));
            File.WriteAllBytes(Map(path), arr);

            return new FileSystemStorageFile(Fix(path), fileInfo);
        }

        public bool IsFileExists(string path)
        {
            return File.Exists(Map(path));
        }

        public bool IsFolderExits(string path)
        {
            return Directory.Exists(Map(path));
        }

        public bool TryCreateFolder(string path)
        {
            try
            {
                if (Directory.Exists(Map(path))) return false;
                CreateFolder(path);
                return true;

                // return false to be consistent with FileSystemProvider's implementation
            }
            catch
            {
                return false;
            }
        }

        public void DeleteFile(string path)
        {
            if (!File.Exists(Map(path)))
            {
                throw new InvalidOperationException("File " + path + " does not exist");
            }

            File.Delete(Map(path));
        }

        public void RenameFile(string path, string newPath)
        {
            if (!File.Exists(Map(path)))
            {
                throw new InvalidOperationException("File " + path + " does not exist");
            }

            if (File.Exists(Map(newPath)))
            {
                throw new InvalidOperationException("File " + newPath + " already exists");
            }

            File.Move(Map(path), Map(newPath));
        }

        internal static bool IsHidden(FileSystemInfo di)
        {
            return (di.Attributes & FileAttributes.Hidden) != 0;
        }

        #endregion
    }
}