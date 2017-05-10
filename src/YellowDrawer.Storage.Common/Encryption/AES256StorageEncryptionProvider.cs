using System.Security.Cryptography;
using System.Text;

namespace YellowDrawer.Storage.Common
{
    public class AesStorageEncryptionProvider : StorageEncryptionProvider
    {
        public AesStorageEncryptionProvider(string key)
        {
            encryptionProvider = new AesCryptoServiceProvider
            {
                KeySize = 256,
                Key = Encoding.UTF8.GetBytes(key)
            };
        }

        
    }
}
