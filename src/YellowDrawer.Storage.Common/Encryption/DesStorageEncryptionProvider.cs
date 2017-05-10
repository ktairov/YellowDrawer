using System.Security.Cryptography;
using System.Text;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Common
{
    public class DesStorageEncryptionProvider : StorageEncryptionProvider
    {
        public DesStorageEncryptionProvider(string key)
        {
            encryptionProvider = DES.Create();
            encryptionProvider.KeySize = 256;
            encryptionProvider.Key = Encoding.UTF8.GetBytes(key);
        }
    }
}
