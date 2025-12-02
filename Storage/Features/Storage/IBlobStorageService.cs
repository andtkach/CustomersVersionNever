namespace Storage.Features;

public interface IBlobStorageService
{
    Task UploadAsync(Guid fileId, Stream data, string contentType, CancellationToken cancellationToken = default);
    Task<(Stream Stream, string ContentType)?> GetAsync(Guid fileId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid fileId, CancellationToken cancellationToken = default);
}
