using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure;
using YellowDrawer.Storage.Azure;
using YellowDrawer.Storage.Common;

namespace YellowDrawer.Storage.Tests.Tests
{
    [TestClass]
    public class AzureTests
    {
        public static string connectionStringAzure = "DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey}";
        private IStorageProvider azureProvider;

        [TestInitialize, TestCategory("Unit")]
        public void Initialize()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionStringAzure);
            azureProvider = new AzureBlobStorageProvider(cloudStorageAccount);
        }

        [TestMethod, TestCategory("Unit")]
        public void SuccessCreateFile()
        {
            CommonTests.SuccessCreateFile(azureProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void SuccessCreateFolder()
        {
            CommonTests.SuccessCreateFolder(azureProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void AnyFolders()
        {
            CommonTests.AnyFolders(azureProvider);
        }

        [TestMethod, TestCategory("Unit")]
        public void AnyFiles()
        {
            CommonTests.AnyFiles(azureProvider);
        }
    }
}
