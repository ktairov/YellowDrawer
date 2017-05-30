using System;
using System.Collections.Generic;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Google
{
    public class GoogleStorageProvider : IStorageProvider
    {
        public IStorageFile CreateFile(string path, byte[] arr = null)
        {
            throw new NotImplementedException();
        }

        public void CreateFolder(string path)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        public void DeleteFolder(string path)
        {
            throw new NotImplementedException();
        }

        public IStorageFile GetFile(string path)
        {
            throw new NotImplementedException();
        }

        public string GetPublicUrl(string path)
        {
            throw new NotImplementedException();
        }

        public bool IsFileExists(string path)
        {
            throw new NotImplementedException();
        }

        public bool IsFolderExits(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStorageFile> ListFiles(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStorageFolder> ListFolders(string path)
        {
            throw new NotImplementedException();
        }

        public void RenameFile(string path, string newPath)
        {
            throw new NotImplementedException();
        }

        public void RenameFolder(string path, string newPath)
        {
            throw new NotImplementedException();
        }
    }
}
