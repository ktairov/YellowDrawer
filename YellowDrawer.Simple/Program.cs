using Amazon.S3;
using YellowDrawer.Storage.AmazonS3;
using YellowDrawer.Storage.Azure;
using YellowDrawer.Storage.Common;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using System.IO;
using YellowDrawer.Storage.GridFS;
using System;
using Microsoft.WindowsAzure;

namespace YellowDrawer.Storage.Simple
{
    class Program
    {
        public static string connectionStringAzure = "DefaultEndpointsProtocol=https;AccountName={AccountName};AccountKey={AccountKey}";

        public static string awsAccessKey = "";
        public static string awsSecretKey = "";
        public static string awsBucketName = "";
        public static string amazonBaseUrl = "TestYellowDrawer";

        public static string mongoConnectionString = "mongodb://{ConnectionString}";
        public static string mongoDataBase = "testDB";
        public static string pathToTestFile = @"..\..\TestFile.jpeg";

        static void Main(string[] args)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionStringAzure);
            var azureProvider = new AzureBlobStorageProvider(cloudStorageAccount);

            var fileSystemProvider = new FileSystemStorageProvider("C://");
            var amazonClient = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USEast1);
            var amazonProvider = new AmazonStorageProvider(amazonClient, amazonBaseUrl, awsBucketName);

            var client = new MongoClient(mongoConnectionString);
            var server = client.GetDatabase(mongoDataBase);
            IGridFSBucket _bucket = new GridFSBucket(server);
            var mongoProvider = new GridFSStorageProvider(_bucket);

            TestProvider(azureProvider, "Azure");
            TestProvider(amazonProvider, "Amazon");
            TestProvider(mongoProvider, "Mongo");
            Console.ReadLine();
        }

        static void TestProvider(IStorageProvider provider, string nameProvider)
        {
            byte[] buff = new byte[0];
            var fileName = pathToTestFile;
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
            Console.WriteLine("********* " + nameProvider + " ****************");
            Console.WriteLine("Create file");
            if (provider.IsFileExists(path))
                Console.WriteLine("File exist");
            else
                Console.WriteLine("File not exist");
            provider.DeleteFile(path);
            Console.WriteLine("Delete file");
        }
    }
}
