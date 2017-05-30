using System;
using System.IO;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Google
{
    public class GoogleStorageFile : IStorageFile
    {
        public string GetFileType()
        {
            throw new NotImplementedException();
        }

        public string GetFullPath()
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastUpdated()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public string GetPath()
        {
            throw new NotImplementedException();
        }

        public long GetSize()
        {
            throw new NotImplementedException();
        }

        public Stream OpenCryptoRead(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            throw new NotImplementedException();
        }

        public Stream OpenCryptoWrite(IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            throw new NotImplementedException();
        }

        public Stream OpenRead()
        {
            throw new NotImplementedException();
        }

        public Stream OpenWrite()
        {
            throw new NotImplementedException();
        }
    }
}
