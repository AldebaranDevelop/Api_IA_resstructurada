using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDrones.infrastructure
{
    public class BlobStorageService
    {
        private readonly string blobStorageConnectionString;
        private readonly string blobContainerName;

        public BlobStorageService(string connectionString, string containerName)
        {
            blobStorageConnectionString = connectionString;
            blobContainerName = containerName;
        }

        public async Task<int> UploadToBlobAsync(Stream imageStream, string blobName, string contentType)
        {
            try
            {
                // Create a BlobServiceClient
                var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

                // Ensure the container exists
                await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);

                // Get the BlobClient for the specified blob
                var blobClient = blobContainerClient.GetBlobClient(blobName);

                // Set headers
                var blobHttpHeaders = new BlobHttpHeaders { ContentType = contentType };

                // Upload the file
                await blobClient.UploadAsync(imageStream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                // Return a success status code (200)
                return 200;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading blob: {ex.Message}");
                // Return a failure status code (500)
                return 500;
            }
        }
    }
}
