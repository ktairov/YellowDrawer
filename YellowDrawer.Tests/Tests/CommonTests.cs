using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Tests.Tests
{
    public static class CommonTests
    {
        public static void SuccessCreateFile(IStorageProvider provider)
        {
            byte[] buff = new byte[0];
            var fileName = @"C:\SOLID\YellowDrawer.Common\YellowDrawer.Simple\TestFile.jpeg";
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);

            var path = "testfolder\\test.jpeg";
            var file = provider.CreateFile(path);
            var streamWrite = file.OpenWrite();
            BinaryWriter bw = new BinaryWriter(streamWrite);
            bw.Write(buff);
            Assert.IsTrue(provider.IsFileExists(path));
            provider.DeleteFile(path);
            Assert.IsFalse(provider.IsFileExists(path));
        }

        public static void SuccessCreateFolder(IStorageProvider provider)
        {
            provider.CreateFolder("TestFolder");
            Assert.IsTrue(provider.IsFolderExits("TestFolder"));
            provider.DeleteFolder("TestFolder");
            Assert.IsFalse(provider.IsFolderExits("TestFolder"));
        }
    }
}
