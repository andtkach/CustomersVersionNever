using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Features.Institution.Services;

public sealed class InstitutionCacheService : IInstitutionCacheService
{
    private readonly HybridCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<InstitutionCacheService> _logger;
    
    private const string InstitutionCacheKeyPrefix = "institution-";
    private const string InstitutionWithCustomersCacheKeyPrefix = "institution-with-customers-";
    private const string InstitutionListCacheKey = "institutions-list";
    private const string InstitutionListWithCustomersCacheKey = "institutions-list-with-customers";


    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromSeconds(30) 
    };

    public InstitutionCacheService(
        HybridCache cache, 
        IHttpClientFactory httpClientFactory,
        ILogger<InstitutionCacheService> logger)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<InstitutionDto?> GetInstitutionAsync(Guid id)
    {
        var cacheKey = $"{InstitutionCacheKeyPrefix}{id}";
        
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var item = await GetInstitutionFromWorkerAsync(id);
                return item;
            },
            CacheOptions);
    }

    public async Task<IEnumerable<InstitutionDto>> GetInstitutionsAsync()
    {
        return await _cache.GetOrCreateAsync(
            InstitutionListCacheKey,
            async _ => await GetInstitutionsFromWorkerAsync(),
            CacheOptions);
    }

    public async Task<IEnumerable<InstitutionWithCustomersDto>> GetInstitutionsWithCustomersAsync()
    {
        return await _cache.GetOrCreateAsync(
            InstitutionListWithCustomersCacheKey,
            async _ => await GetInstitutionsWithCustomersFromWorkerAsync(),
            CacheOptions);
    }

    public async Task<InstitutionWithCustomersDto?> GetInstitutionWithCustomersAsync(Guid id)
    {
        var cacheKey = $"{InstitutionWithCustomersCacheKeyPrefix}{id}";

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var item = await GetInstitutionWithCustomersFromWorkerAsync(id);
                return item;
            },
            CacheOptions);
    }

    private async Task<InstitutionDto?> GetInstitutionFromWorkerAsync(Guid id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync($"institutions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var institution = await response.Content.ReadFromJsonAsync<InstitutionDto>();
                return institution;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning(
                "Failed to get institution {InstitutionId}. Status: {StatusCode}",
                id,
                response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Institutions API for institution {InstitutionId}", id);
            return null;
        }
    }

    private async Task<IEnumerable<InstitutionDto>> GetInstitutionsFromWorkerAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync("institutions");

            if (response.IsSuccessStatusCode)
            {
                var institutions = await response.Content.ReadFromJsonAsync<IEnumerable<InstitutionDto>>();
                return institutions ?? [];
            }

            _logger.LogWarning(
                "Failed to get institutions list. Status: {StatusCode}",
                response.StatusCode);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Institutions API for institutions list");
            return [];
        }
    }

    private async Task<IEnumerable<InstitutionWithCustomersDto>> GetInstitutionsWithCustomersFromWorkerAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync("institutions?includeCustomers=true");

            if (response.IsSuccessStatusCode)
            {
                var institutions = await response.Content.ReadFromJsonAsync<IEnumerable<InstitutionWithCustomersDto>>();
                return institutions ?? [];
            }

            _logger.LogWarning(
                "Failed to get institutions list. Status: {StatusCode}",
                response.StatusCode);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Institutions API for institutions list");
            return [];
        }
    }

    private async Task<InstitutionWithCustomersDto?> GetInstitutionWithCustomersFromWorkerAsync(Guid id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync($"institutions/{id}?includeCustomers=true");

            if (response.IsSuccessStatusCode)
            {
                var institution = await response.Content.ReadFromJsonAsync<InstitutionWithCustomersDto>();
                return institution;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning(
                "Failed to get institution with customers {InstitutionId}. Status: {StatusCode}",
                id,
                response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Institutions API for institution with customers {InstitutionId}", id);
            return null;
        }
    }
}