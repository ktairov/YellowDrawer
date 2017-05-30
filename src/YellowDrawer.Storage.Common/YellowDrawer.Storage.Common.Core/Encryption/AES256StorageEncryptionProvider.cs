using System.Security.Cryptography;
using System.Text;

namespace YellowDrawer.Storage.Common
{
    public class AesStorageEncryptionProvider : StorageEncryptionProvider
    {
        public AesStorageEncryptionProvider(string key)
        {
            encryptionProvider = Aes.Create();
            encryptionProvider.KeySize = 256;
            encryptionProvider.Key = Encoding.UTF8.GetBytes(key);
        }
    }
}
