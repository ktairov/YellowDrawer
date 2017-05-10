using System.Security.Cryptography;
using System.Text;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Common.Encryption
{
    public class Rc2StorageEncryptionProvider : StorageEncryptionProvider
    {
        public Rc2StorageEncryptionProvider(string key)
        {
            encryptionProvider = new RC2CryptoServiceProvider();
            encryptionProvider.KeySize = 256;
            encryptionProvider.Key = Encoding.UTF8.GetBytes(key);
        }
    }
}
