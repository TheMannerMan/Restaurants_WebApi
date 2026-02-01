using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Restaurants.Domain.Interfaces;
using Restaurants.Infrastructure.Configuration;

namespace Restaurants.Infrastructure.Storage;

// IOptions<BlobStorageSettings> is injected by the DI container and provides access to the 
// BlobStorageSettings configuration that was bound in ServiceCollectionExtensions using 
// services.Configure<BlobStorageSettings>(...). The .Value property returns the actual 
// BlobStorageSettings object with values populated from appsettings.json.
internal class BlobStorageService(IOptions<BlobStorageSettings> blobStorageSettingsOptions) : IBlobStorageService
{
    private readonly BlobStorageSettings _blobStorageSettings = blobStorageSettingsOptions.Value;
    public async Task<string> UploadToBlobAsync(Stream data, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.LogosContainerName);

        // GetBlobClient returns a reference to a blob with the specified fileName (doesn't create it yet).
        // Note: If a blob with this name already exists, UploadAsync will OVERWRITE it by default.
        // Consider using unique filenames (e.g., GUID + original name) to prevent accidental overwrites.
        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(data);

        var blobUrl = blobClient.Uri.ToString();
        return blobUrl;
    }

    public string? GetBlobSasUrl(string? blobUrl)
    {
        if (blobUrl == null) return null;

        // Build a Shared Access Signature (SAS) token for time-limited, secure blob access
        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _blobStorageSettings.LogosContainerName,
            Resource = "b", // "b" indicates the resource is a blob
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30),
            BlobName = GetBlobNameFromUrl(blobUrl)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);

        var sasToken = sasBuilder
            .ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(blobServiceClient.AccountName, _blobStorageSettings.AccountKey))
            .ToString();
        return $"{blobUrl}?{sasToken}";

        //Example output:
        //blob: https://account.blob.core.windows.net/logos/mylogo.png
        // sasToken: sv=2024-02-14&st=2024-06-10T12%3A00%3A00Z&se=2024-06-10T12%3A30%3A00Z&sr=b&sp=r&sig=...
    }

    private string GetBlobNameFromUrl(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        // Uri.Segments splits the URL path into an array of segments (e.g., "/", "container/", "filename.png").
        // .Last() retrieves the final segment, which is the blob name/filename.
        // Example: "https://account.blob.core.windows.net/logos/mylogo.png" → "mylogo.png"
        return uri.Segments.Last(); 
    }
}
