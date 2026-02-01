namespace Restaurants.Domain.Interfaces;

public interface IBlobStorageService
{
    string? GetBlobSasUrl(string? blobUrl);

    // the returned string is the URL of the uploaded blob
    Task<string> UploadToBlobAsync(Stream data, string fileName);
}
