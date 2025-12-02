using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Storage.Features;

internal sealed class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "files";

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    private async Task<BlobContainerClient> GetOrCreateContainerAsync(CancellationToken ct)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct).ConfigureAwait(false);
        return container;
    }

    public async Task UploadAsync(Guid fileId, Stream data, string contentType, CancellationToken cancellationToken = default)
    {
        var container = await GetOrCreateContainerAsync(cancellationToken).ConfigureAwait(false);
        var blobClient = container.GetBlobClient(fileId.ToString("D"));
        data.Position = 0;
        var headers = new BlobHttpHeaders { ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType };
        await blobClient.UploadAsync(data, headers, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<(Stream Stream, string ContentType)?> GetAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = container.GetBlobClient(fileId.ToString("D"));

        var exists = await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
        if (!exists)
        {
            return null;
        }

        var props = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        var contentType = props.Value.ContentType ?? "application/octet-stream";

        var stream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return (stream, contentType);
    }

    public async Task<bool> DeleteAsync(Guid fileId, CancellationToken cancellationToken = default)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = container.GetBlobClient(fileId.ToString("D"));
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return response.Value;
    }
}
