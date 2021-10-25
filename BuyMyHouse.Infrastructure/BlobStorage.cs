using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyMyHouse.Infrastructure
{
    public class BlobStorage : IBlobStorage
    {
        private CloudStorageAccount cloudStorageAccount { get; set; }
        private string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        public BlobStorage()
        {
            cloudStorageAccount = GetCloudStorageAccount();
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            if (cloudStorageAccount == null)
            {
                cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            }
            return cloudStorageAccount;
        }

        public CloudBlobContainer GetContainerReference(string containerName)
        {
            cloudStorageAccount = GetCloudStorageAccount();
            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);
        }

        public async Task<bool> UploadImage(string imageReferenceName, Stream image)
        {
            try
            {
                var cloudBlobContainer = GetContainerReference("images");
                CloudBlockBlob cBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageReferenceName);
                cBlockBlob.Properties.ContentType = "image/png";
                await cBlockBlob.UploadFromStreamAsync(image);
                return true;
            }
            catch (StorageException ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public async Task<string> GetImage(string imageReferenceName)
        {
            try
            {
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("images");
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageReferenceName);
                return cloudBlockBlob.Uri.ToString();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<bool> UploadPdf(string pdfRefName, Stream pdf)
        {
            try
            {
                var cloudBlobContainer = GetContainerReference("pdf");
                CloudBlockBlob cBlockBlob = cloudBlobContainer.GetBlockBlobReference(pdfRefName);
                cBlockBlob.Properties.ContentType = "text/plain"; // application/pdf
                // Set the CacheControl property to expire in 1 hour (3600 seconds)
                cBlockBlob.Properties.CacheControl = "max-age=3600";
                await cBlockBlob.UploadFromStreamAsync(pdf);
                return true;
            }
            catch (StorageException ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public async Task<string> GetPdf(string pdfRefName)
        {
            try
            {
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("pdf");
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(pdfRefName);
                return cloudBlockBlob.Uri.ToString();
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
    }
}   
