using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Features.Address.Services;

public interface IAddressCacheService
{
    Task<AddressDto?> GetAddressAsync(Guid id);
    Task<IEnumerable<AddressDto>> GetAddressesAsync();
}

public sealed class AddressCacheService : IAddressCacheService
{
    private readonly HybridCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AddressCacheService> _logger;
    
    private const string AddressCacheKeyPrefix = "address-";
    private const string AddressListCacheKey = "addresses-list";
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromSeconds(30) 
    };

    public AddressCacheService(
        HybridCache cache, 
        IHttpClientFactory httpClientFactory,
        ILogger<AddressCacheService> logger)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<AddressDto?> GetAddressAsync(Guid id)
    {
        var cacheKey = $"{AddressCacheKeyPrefix}{id}";
        
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var item = await GetAddressFromWorkerAsync(id);
                return item;
            },
            CacheOptions);
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesAsync()
    {
        return await _cache.GetOrCreateAsync(
            AddressListCacheKey,
            async _ => await GetAddressesFromWorkerAsync(),
            CacheOptions);
    }

    private async Task<AddressDto?> GetAddressFromWorkerAsync(Guid id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync($"addresses/{id}");

            if (response.IsSuccessStatusCode)
            {
                var document = await response.Content.ReadFromJsonAsync<AddressDto>();
                return document;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning(
                "Failed to get address {AddressId}. Status: {StatusCode}",
                id,
                response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Customers API for address {AddressId}", id);
            return null;
        }
    }

    private async Task<IEnumerable<AddressDto>> GetAddressesFromWorkerAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var response = await httpClient.GetAsync("addresses");

            if (response.IsSuccessStatusCode)
            {
                var addresses = await response.Content.ReadFromJsonAsync<IEnumerable<AddressDto>>();
                return addresses ?? [];
            }

            _logger.LogWarning(
                "Failed to get addresses list. Status: {StatusCode}",
                response.StatusCode);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Customers API for address list");
            return [];
        }
    }
}