using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Tests.Tests
{
    public static class CommonTests
    {
        public static void SuccessCreateFile(IStorageProvider provider)
        {
            byte[] buff = new byte[0];
            var fileName = @"..\..\TestFile.jpeg";
            var fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            var br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);

            var path = "testfolder\\test.jpeg";
            var file = provider.CreateFile(path);
            var streamWrite = file.OpenWrite();
            var bw = new BinaryWriter(streamWrite);
            bw.Write(buff);
            Assert.IsTrue(provider.IsFileExists(path));
            provider.DeleteFile(path);
            Assert.IsFalse(provider.IsFileExists(path));
        }

        public static void SuccessCreateFolder(IStorageProvider provider)
        {
            if (provider.IsFolderExits("container\\TestFolder"))
                provider.DeleteFolder("container\\TestFolder");
            provider.CreateFolder("container\\TestFolder");
            Assert.IsTrue(provider.IsFolderExits("container\\TestFolder"));
            provider.DeleteFolder("container\\TestFolder");
            Assert.IsFalse(provider.IsFolderExits("container\\TestFolder"));
        }

        public static void AnyFolders(IStorageProvider provider)
        {
            if(provider.IsFolderExits("container\\TestFolder\\TestFolder"))
                provider.DeleteFolder("container\\TestFolder\\TestFolder");
            provider.CreateFolder("container\\TestFolder\\TestFolder");
            Assert.IsTrue(provider.ListFolders("container\\TestFolder\\").Any());
            provider.DeleteFolder("container\\TestFolder\\TestFolder");
        }

        public static void AnyFiles(IStorageProvider provider)
        {
            byte[] buff = new byte[0];
            var fileName = @"..\..\TestFile.jpeg";
            var fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            var br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);

            var path = "container\\TestFolder\\test.jpeg";
            var file = provider.CreateFile(path);
            var streamWrite = file.OpenWrite();
            var bw = new BinaryWriter(streamWrite);
            bw.Write(buff);
            Assert.IsTrue(provider.ListFiles("container\\TestFolder\\").Any());
            provider.DeleteFile(path);
        }
    }
}
