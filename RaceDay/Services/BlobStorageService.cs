
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Services;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString =
            configuration["AzureBlobStorage:ConnectionString"];

        var containerName =
            configuration["AzureBlobStorage:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString) ||
            string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException(
                "Azure Blob Storage configuration is missing.");
        }

        var blobServiceClient =
            new BlobServiceClient(connectionString);

        _containerClient =
            blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadProfilePictureAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        return await UploadFileAsync(
            fileStream,
            fileName,
            contentType,
            "profiles");
    }

    public async Task<string> UploadEventBannerAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        return await UploadFileAsync(
            fileStream,
            fileName,
            contentType,
            "event-banners");
    }

    private async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folder)
    {
        await _containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None);

        var blobName =
            $"{folder}/{Guid.NewGuid()}-{Path.GetFileName(fileName)}";

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders
            {
                ContentType = contentType
            });

        return blobName;
    }

    public async Task<BlobDownloadStreamingResult?> DownloadAsync(
        string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return null;
        }

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        var exists = await blobClient.ExistsAsync();

        if (!exists.Value)
        {
            return null;
        }

        var response =
            await blobClient.DownloadStreamingAsync();

        return response.Value;
    }
}
