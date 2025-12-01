using Worker.Data.Model;

namespace Worker.Cache;

public interface IDocumentCacheService
{
    Task CacheDocumentAsync(Document document, string company);
    Task ClearDocumentAsync(Guid documentId, string company);
    Task ClearDocumentsListAsync(string company);
}