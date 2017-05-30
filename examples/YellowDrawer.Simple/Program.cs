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
        public static string connectionStringAzure = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";

        public static string awsAccessKey = "";
        public static string awsSecretKey = "";
        public static string awsBucketName = "";
        public static string amazonBaseUrl = "TestYellowDrawer";

        public static string mongoConnectionString = "mongodb://{ConnectionString}";
        public static string mongoDataBase = "testDB";
        public static string pathToTestFile = @"..\..\TestFile.jpeg";

        static void Main(string[] args)
        {
            var encryptionProvider = new AesStorageEncryptionProvider("cuwRX6tRedZGFlAG2vOBytIS4iaWOv8D");

            var cloudStorageAccount = CloudStorageAccount.Parse(connectionStringAzure);
            var azureProvider = new AzureBlobStorageProvider(cloudStorageAccount);

            var fileSystemProvider = new FileSystemStorageProvider("C://");
            var amazonClient = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USEast1);
            var amazonProvider = new AmazonStorageProvider(amazonClient, amazonBaseUrl, awsBucketName);

            var client = new MongoClient(mongoConnectionString);
            var server = client.GetDatabase(mongoDataBase);
            IGridFSBucket _bucket = new GridFSBucket(server);
            var mongoProvider = new GridFSStorageProvider(_bucket);

            var iv = encryptionProvider.GenerateIV();
            TestWriteEncryptionProvider(azureProvider, encryptionProvider, iv);
            TestReadEncryptionProvider(azureProvider, encryptionProvider, iv);

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

        static void TestWriteEncryptionProvider(IStorageProvider provider, IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            byte[] buff = new byte[0];
            var fileName = pathToTestFile;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);

            var path = "testfolder\\testencryption990.jpeg";
            provider.DeleteFile(path);
            var file = provider.CreateFile(path);
            var streamWrite = file.OpenCryptoWrite(encryptionProvider, iv);
            CopyStream(fs, streamWrite);
        }

        static void TestReadEncryptionProvider(IStorageProvider provider, IStorageEncryptionProvider encryptionProvider, byte[] iv)
        {
            var path = "testfolder\\testencryption990.jpeg";
            var file = provider.GetFile(path);
            var streamRead = file.OpenCryptoRead(encryptionProvider, iv);
            
            FileStream fs = new FileStream(@"..\..\TestDecryptFile990.jpeg",
                                           FileMode.OpenOrCreate,
                                           FileAccess.Write);

            CopyStream(streamRead, fs);
        }

        private static void CopyStream(Stream input, Stream output)
        {
            using (output)
            {
                using (input)
                {
                    byte[] numArray = new byte[9192];
                    while (true)
                    {
                        int num = input.Read(numArray, 0, numArray.Length);
                        int num1 = num;
                        if (num <= 0)
                        {
                            break;
                        }
                        output.Write(numArray, 0, num1);
                    }
                }
            }
        }
    }
}
