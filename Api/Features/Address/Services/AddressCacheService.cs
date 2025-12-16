using Common.Authorization;
using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Features.Address.Services;

public sealed class AddressCacheService : IAddressCacheService
{
    private readonly HybridCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AddressCacheService> _logger;
    private readonly UserHelper _userHelper;

    private const string AddressCacheKeyPrefix = "address-";
    private const string AddressListCacheKey = "addresses-list";
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromSeconds(30) 
    };

    public AddressCacheService(
        HybridCache cache, 
        IHttpClientFactory httpClientFactory,
        ILogger<AddressCacheService> logger,
        UserHelper userHelper)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _userHelper = userHelper;
    }

    public async Task<AddressDto?> GetAddressAsync(Guid id)
    {
        var cacheKey = $"{AddressCacheKeyPrefix}{id}{_userHelper.GetCompanyForCache()}";
        
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
        var cacheKey = $"{AddressListCacheKey}{_userHelper.GetCompanyForCache()}";
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ => await GetAddressesFromWorkerAsync(),
            CacheOptions);
    }

    public async Task Invalidate(Guid id)
    {
        await _cache.RemoveAsync($"{AddressCacheKeyPrefix}{id}{_userHelper.GetCompanyForCache()}");
        await _cache.RemoveAsync($"{AddressListCacheKey}{_userHelper.GetCompanyForCache()}");
    }

    public async Task PutNewAddress(AddressDto addressDto)
    {
        await _cache.SetAsync($"{AddressCacheKeyPrefix}{addressDto.Id}{_userHelper.GetCompanyForCache()}", addressDto);
    }

    private async Task<AddressDto?> GetAddressFromWorkerAsync(Guid id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var userCompanyId = _userHelper.GetUserCompany();
            httpClient.DefaultRequestHeaders.Add("company", userCompanyId);

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
            var userCompanyId = _userHelper.GetUserCompany();
            httpClient.DefaultRequestHeaders.Add("company", userCompanyId);

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