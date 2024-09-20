using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Threading.Tasks;

namespace  Sharesafely.Services;
public class BlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadBlobsAsync(string containerName, Dictionary<string, Stream> files)
    {
        // Get a reference to the container
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
    
        // Create the container if it doesn't exist
        await containerClient.CreateIfNotExistsAsync();
        // Generate a unique folder name using a GUID
        string uniqueFolder = Guid.NewGuid().ToString();
        
        foreach (var file in files)
        {
            var blobName = file.Key;
            var data = file.Value;

            // Check if the stream length is less than 20 MB
            if (data.Length > 20 * 1024 * 1024)
            {
                throw new InvalidOperationException($"File {blobName} exceeds the 20 MB limit.");
            }
            // Update blob name to include the folder path
            string blobPath = $"{uniqueFolder}/{blobName}";

            // Get a reference to the blob
            var blobClient = containerClient.GetBlobClient(blobPath);

            // Upload the blob
            await blobClient.UploadAsync(data, overwrite: true);
        }
    }

}