using Common.Authorization;
using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class DocumentCacheService(HybridCache cache, UserHelper userHelper) : IDocumentCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheDocumentAsync(Document document, string company)
    {
        var cacheKey = $"document-{document.Id}{userHelper.NormaliseCompanyForCache(company)}";
        var item = new DocumentDto(document.Id, document.CustomerId, document.Title, document.Content, document.Active);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearDocumentAsync(Guid documentId, string company)
    {
        var cacheKey = $"document-{documentId}{userHelper.NormaliseCompanyForCache(company)}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearDocumentsListAsync(string company)
    {
        var cacheKey = $"documents-list{userHelper.NormaliseCompanyForCache(company)}";
        await cache.RemoveAsync(cacheKey);
    }
}
