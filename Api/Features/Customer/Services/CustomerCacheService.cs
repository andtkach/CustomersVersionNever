using Common.Authorization;
using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Features.Customer.Services;

public sealed class CustomerCacheService : ICustomerCacheService
{
    private readonly HybridCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CustomerCacheService> _logger;
    private readonly UserHelper _userHelper;

    private const string CustomerCacheKeyPrefix = "customer-";
    private const string CustomerListCacheKey = "customers-list";
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromSeconds(30) 
    };

    public CustomerCacheService(
        HybridCache cache, 
        IHttpClientFactory httpClientFactory,
        ILogger<CustomerCacheService> logger,
        UserHelper userHelper)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _userHelper = userHelper;
    }

    public async Task<CustomerDto?> GetCustomerAsync(Guid id)
    {
        var cacheKey = $"{CustomerCacheKeyPrefix}{id}{_userHelper.GetCompanyForCache()}";
        
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ =>
            {
                var item = await GetCustomerFromWorkerAsync(id);
                return item;
            },
            CacheOptions);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
    {
        var cacheKey = $"{CustomerListCacheKey}{_userHelper.GetCompanyForCache()}";
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async _ => await GetCustomersFromWorkerAsync(),
            CacheOptions);
    }

    public async Task Invalidate(Guid id)
    {
        await _cache.RemoveAsync($"{CustomerCacheKeyPrefix}{id}{_userHelper.GetCompanyForCache()}");
        await _cache.RemoveAsync($"{CustomerListCacheKey}{_userHelper.GetCompanyForCache()}");
    }
    
    private async Task<CustomerDto?> GetCustomerFromWorkerAsync(Guid id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var userCompanyId = _userHelper.GetUserCompany();
            httpClient.DefaultRequestHeaders.Add("company", userCompanyId);
            
            var response = await httpClient.GetAsync($"customers/{id}");

            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
                return customer;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            _logger.LogWarning(
                "Failed to get customer {CustomerId}. Status: {StatusCode}",
                id,
                response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Customers API for customer {CustomerId}", id);
            return null;
        }
    }

    private async Task<IEnumerable<CustomerDto>> GetCustomersFromWorkerAsync()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("WorkerApi");
            var userCompanyId = _userHelper.GetUserCompany();
            httpClient.DefaultRequestHeaders.Add("company", userCompanyId);
            
            var response = await httpClient.GetAsync("customers");

            if (response.IsSuccessStatusCode)
            {
                var customers = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerDto>>();
                return customers ?? [];
            }

            _logger.LogWarning(
                "Failed to get customers list. Status: {StatusCode}",
                response.StatusCode);
            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Customers API for customer list");
            return [];
        }
    }
}