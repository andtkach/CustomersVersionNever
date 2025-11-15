using Microsoft.Extensions.Caching.Hybrid;
using Common.Dto;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class DocumentCacheService(HybridCache cache) : IDocumentCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheDocumentAsync(Document document)
    {
        var cacheKey = $"document-{document.Id}";
        var item = new DocumentDto(document.Id, document.CustomerId, document.Title, document.Content, document.Active);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearDocumentAsync(Guid documentId)
    {
        var cacheKey = $"document-{documentId}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearDocumentsListAsync()
    {
        var cacheKey = "documents-list";
        await cache.RemoveAsync(cacheKey);
    }
}
