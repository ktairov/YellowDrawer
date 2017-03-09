using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using YellowDrawer.Storage.Common;
using YellowDrawer.Storage.GridFS;
using YellowDrawer.Storage.Tests.Tests;

namespace YellowDrawer.Tests.Tests
{
    [TestClass]
    public class GridFSTests
    {
        public static string mongoConnectionString = "mongodb://{mongoConnectionString}";
        public static string mongoDataBase = "testDB";
        private IStorageProvider mongoProvider;

        [TestInitialize, TestCategory("Unit")]
        public void Initialize()
        {
            var client = new MongoClient(mongoConnectionString);
            var server = client.GetDatabase(mongoDataBase);
            var _bucket = new GridFSBucket(server);
            mongoProvider = new GridFSStorageProvider(_bucket);
        }

        [TestMethod, TestCategory("Unit")]
        public void SuccessCreateFile()
        {
            CommonTests.SuccessCreateFile(mongoProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void SuccessCreateFolder()
        {
            CommonTests.SuccessCreateFolder(mongoProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void AnyFolders()
        {
            CommonTests.AnyFolders(mongoProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void AnyFiles()
        {
            CommonTests.AnyFiles(mongoProvider);
        }
    }
}
