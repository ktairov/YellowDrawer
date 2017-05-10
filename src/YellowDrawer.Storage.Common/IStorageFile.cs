using System;
using System.IO;

namespace YellowDrawer.Storage.Common
{
    public interface IStorageFile
    {
        string GetPath();
        string GetFullPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        string GetFileType();

        Stream OpenRead();
        Stream OpenWrite();

        Stream OpenCryptoRead(IStorageEncryptionProvider encryptionProvider, byte[] iv);
        Stream OpenCryptoWrite(IStorageEncryptionProvider encryptionProvider, byte[] iv);
    }
}