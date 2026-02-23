using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApiDrones.infrastructure
{
    public class BlobStorageService
    {
        private readonly string blobStorageConnectionString;
        private readonly string blobContainerName;

        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorageService(string connectionString, string containerName)
        {
            blobStorageConnectionString = connectionString;
            blobContainerName = containerName;

            var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);

            _blobContainerClient =
                blobServiceClient.GetBlobContainerClient(blobContainerName);

            _blobContainerClient.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<int> UploadToBlobAsync(
            Stream imageStream,
            string blobName,
            string contentType)
        {
            try
            {
                var blobClient = _blobContainerClient.GetBlobClient(blobName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                await blobClient.UploadAsync(
                    imageStream,
                    new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

                return 200;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading blob: {ex.Message}");
                return 500;
            }
        }
    }
}