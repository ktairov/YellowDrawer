using System.IO;
using System.Security.Cryptography;

namespace YellowDrawer.Storage.Common
{
    public abstract class StorageEncryptionProvider : IStorageEncryptionProvider
    {
        protected SymmetricAlgorithm encryptionProvider;
        public byte[] IV { get { return encryptionProvider.IV; } }
        public byte[] GenerateIV()
        {
            encryptionProvider.GenerateIV();
            return encryptionProvider.IV;
        }

        public Stream Decrypt(Stream stream, byte[] iv)
        {
            encryptionProvider.IV = iv;
            return new CryptoStream(stream, encryptionProvider.CreateDecryptor(), CryptoStreamMode.Read);
        }

        public Stream Encrypt(Stream stream, byte[] iv)
        {
            encryptionProvider.IV = iv;
            return new CryptoStream(stream, encryptionProvider.CreateEncryptor(), CryptoStreamMode.Write);
        }
    }
}
