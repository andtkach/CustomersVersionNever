using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Features.Document.Services;

public interface IDocumentCacheService
{
    Task<DocumentDto?> GetDocumentAsync(Guid id);
    Task<IEnumerable<DocumentDto>> GetDocumentsAsync();
}

public sealed class DocumentCacheService : IDocumentCacheService
{
    private readonly HybridCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DocumentCacheService> _logger;
    
    private const string DocumentCacheKeyPrefix = "document-";
    private const string DocumentListCacheKey = "documents-list";
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromSeconds(30) 
    };

    public DocumentCacheService(
        HybridCache cache, 
        IHttpClientFactory httpClientFactory,
        ILogger<DocumentCacheService> logger)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<DocumentDto?> GetDocumentAsync(Guid id)
    {
        var cacheKey = $"{DocumentCacheKeyPrefix}{id}";
        
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var item = await GetDocumentFromWorkerAsync(id);
                return item;
            },
            CacheOptions);
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsAsync()
    {
        return await _cache.GetOrCreateAsync(
            DocumentListCacheKey,
            async _ => await GetDocumentsFromWorkerAsync(),
            CacheOptions);
    }

    private async Task<DocumentDto?> GetDocumentFromWorkerAsync(Guid id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync($"documents/{id}");

            if (response.IsSuccessStatusCode)
            {
                var document = await response.Content.ReadFromJsonAsync<DocumentDto>();
                return document;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning(
                "Failed to get document {DocumentId}. Status: {StatusCode}",
                id,
                response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Customers API for document {DocumentId}", id);
            return null;
        }
    }

    private async Task<IEnumerable<DocumentDto>> GetDocumentsFromWorkerAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync("documents");

            if (response.IsSuccessStatusCode)
            {
                var documents = await response.Content.ReadFromJsonAsync<IEnumerable<DocumentDto>>();
                return documents ?? [];
            }

            _logger.LogWarning(
                "Failed to get documents list. Status: {StatusCode}",
                response.StatusCode);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Customers API for document list");
            return [];
        }
    }
}