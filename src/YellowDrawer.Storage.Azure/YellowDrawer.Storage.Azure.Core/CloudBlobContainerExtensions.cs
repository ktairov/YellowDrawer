﻿using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;

namespace YellowDrawer.Storage.Azure
{
    public static class CloudBlobContainerExtensions
    {
        public static bool BlobExists(this CloudBlobContainer container, string path)
        {
            if (string.IsNullOrEmpty(path) || path.Trim() == string.Empty)
                throw new ArgumentException("Path can't be empty");

            return container.GetBlockBlobReference(path.Replace("\\", "/")).ExistsAsync().Result;
        }

        public static void EnsureBlobExists(this CloudBlobContainer container, string path)
        {
            if (!BlobExists(container, path))
            {
                throw new ArgumentException("File " + path + " does not exist");
            }
        }

        public static void EnsureBlobDoesNotExist(this CloudBlobContainer container, string path)
        {
            if (BlobExists(container, path))
            {
                throw new ArgumentException("File " + path + " already exists");
            }
        }

        public static bool DirectoryExists(this CloudBlobContainer container, string path)
        {
            if (string.IsNullOrEmpty(path) || path.Trim() == string.Empty)
                throw new ArgumentException("Path can't be empty");

            return container.GetDirectoryReference(path).ListBlobsSegmentedAsync(null).Result.Results.Count() > 0;
        }

        public static void EnsureDirectoryExists(this CloudBlobContainer container, string path)
        {
            if (!DirectoryExists(container, path))
            {
                throw new ArgumentException("Directory " + path + " does not exist");
            }
        }

        public static void EnsureDirectoryDoesNotExist(this CloudBlobContainer container, string path)
        {
            if (DirectoryExists(container, path))
            {
                throw new ArgumentException("Directory " + path + " already exists");
            }
        }
    }
}