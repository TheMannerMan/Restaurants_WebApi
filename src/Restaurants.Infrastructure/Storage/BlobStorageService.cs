using Azure.Storage.Blobs;
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
}
