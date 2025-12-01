using Common.Authorization;
using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Features.Document.Services;

public sealed class DocumentCacheService : IDocumentCacheService
{
    private readonly HybridCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DocumentCacheService> _logger;
    private readonly UserHelper _userHelper;

    private const string DocumentCacheKeyPrefix = "document-";
    private const string DocumentListCacheKey = "documents-list";
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromSeconds(30) 
    };

    public DocumentCacheService(
        HybridCache cache, 
        IHttpClientFactory httpClientFactory,
        ILogger<DocumentCacheService> logger,
        UserHelper userHelper)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _userHelper = userHelper;
    }

    public async Task<DocumentDto?> GetDocumentAsync(Guid id)
    {
        var cacheKey = $"{DocumentCacheKeyPrefix}{id}{_userHelper.GetCompanyForCache()}";
        
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
        var cacheKey = $"{DocumentListCacheKey}{_userHelper.GetCompanyForCache()}";
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ => await GetDocumentsFromWorkerAsync(),
            CacheOptions);
    }

    public async Task Invalidate(Guid id)
    {
        await _cache.RemoveAsync($"{DocumentCacheKeyPrefix}{id}{_userHelper.GetCompanyForCache()}");
        await _cache.RemoveAsync($"{DocumentListCacheKey}{_userHelper.GetCompanyForCache()}");
    }

    private async Task<DocumentDto?> GetDocumentFromWorkerAsync(Guid id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var userCompanyId = _userHelper.GetUserCompany();
            httpClient.DefaultRequestHeaders.Add("company", userCompanyId); 
            
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
            var userCompanyId = _userHelper.GetUserCompany();
            httpClient.DefaultRequestHeaders.Add("company", userCompanyId);

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