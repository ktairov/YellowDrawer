# .NET SDK for YellowDrawer.Storage.


[![Build status](https://ci.appveyor.com/api/projects/status/t76g9ja3s5a3ex1q?svg=true)](https://ci.appveyor.com/project/AlexeyKharchenko/yellowdrawer) [![NuGet](https://img.shields.io/nuget/v/YellowDrawer.Storage.Common.svg)](https://www.nuget.org/packages/YellowDrawer.Storage.Common/) 

## What is YellowDrawer.Storage?

.NET library to access GridFS, Azure or Amazon file storage.

## Install

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [YellowDrawer Azure](https://www.nuget.org/packages/YellowDrawer.Storage.Azure/) or [YellowDrawer GridFS](https://www.nuget.org/packages/YellowDrawer.Storage.GridFS/) or [YellowDrawer Amazon](https://www.nuget.org/packages/YellowDrawer.Storage.Amazon/) from the package manager console:

```
PM> YellowDrawer.Storage.Azure
```

```
PM> YellowDrawer.Storage.Amazon
```

```
PM> YellowDrawer.Storage.GridFS
```

The latest versions of the required frameworks are automatically installed 

## Initialize Azure StorageProvider

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			var cloudStorageAccount = CloudStorageAccount.Parse(connectionStringAzure);
			azureProvider = new AzureBlobStorageProvider(cloudStorageAccount);
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Initialize Amazon StorageProvider

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			var awsAccessKey = "";
			var awsSecretKey = "";
			var awsBucketName = "";
			var amazonBaseUrl = "TestYellowDrawer";
			var amazonClient = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USEast1);
			var amazonProvider = new AmazonStorageProvider(amazonClient, amazonBaseUrl, awsBucketName);
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Initialize GridFS StorageProvider

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			var mongoConnectionString = "mongodb://{ConnectionString}";
			var client = new MongoClient(mongoConnectionString);
			var server = client.GetDatabase(mongoDataBase);
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

## Initialize FileSystem StorageProvider

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			var fileSystemProvider = new FileSystemStorageProvider("C://");
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


## Actions StorageProvider
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			GetPublicUrl(string path);
			IStorageFile GetFile(string path);
			IEnumerable<IStorageFile> ListFiles(string path);
			IEnumerable<IStorageFolder> ListFolders(string path);
			void CreateFolder(string path);
			void DeleteFolder(string path);
			void RenameFolder(string path, string newPath);
			void DeleteFile(string path);
			void RenameFile(string path, string newPath);
			IStorageFile CreateFile(string path, byte[] arr = null);
			bool IsFileExists(string path);
			bool IsFolderExits(string path);
			bool TryCreateFolder(string path);
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~