using Worker.Data.Model;

namespace Worker.Cache;

public interface IDocumentCacheService
{
    Task CacheDocumentAsync(Document document);
    Task ClearDocumentAsync(Guid documentId);
    Task ClearDocumentsListAsync();
}