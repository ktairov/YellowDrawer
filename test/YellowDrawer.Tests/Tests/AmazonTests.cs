using Amazon.S3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowDrawer.Storage.AmazonS3;
using YellowDrawer.Storage.Common;
using YellowDrawer.Storage.Tests.Tests;

namespace YellowDrawer.Tests.Tests
{
    [TestClass]
    public class AmazonTests
    {
        public static string awsAccessKey = "";
        public static string awsSecretKey = "";
        public static string awsBucketName = "";
        public static string amazonBaseUrl = "TestYellowDrawer";
        private IStorageProvider amazonProvider;

        [TestInitialize, TestCategory("Unit")]
        public void Initialize()
        {
            var amazonClient = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USEast1);
            amazonProvider = new AmazonStorageProvider(amazonClient, amazonBaseUrl, awsBucketName);
        }

        [TestMethod, TestCategory("Unit")]
        public void SuccessCreateFile()
        {
            CommonTests.SuccessCreateFile(amazonProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void SuccessCreateFolder()
        {
            CommonTests.SuccessCreateFolder(amazonProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void AnyFolders()
        {
            CommonTests.AnyFolders(amazonProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void AnyFiles()
        {
            CommonTests.AnyFiles(amazonProvider);
        }
    }
}
