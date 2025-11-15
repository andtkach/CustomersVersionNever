using Microsoft.Extensions.Caching.Hybrid;
using Common.Dto;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class CustomerCacheService(HybridCache cache) : ICustomerCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheCustomerAsync(Customer customer)
    {
        var cacheKey = $"customer-{customer.Id}";
        var item = new CustomerDto(customer.Id, customer.InstitutionId, customer.FirstName, customer.LastName);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearCustomerAsync(Guid customerId)
    {
        var cacheKey = $"customer-{customerId}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearCustomersListAsync()
    {
        var cacheKey = "customers-list";
        await cache.RemoveAsync(cacheKey);
    }
}
