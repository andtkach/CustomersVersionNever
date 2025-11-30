using Common.Authorization;
using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class CustomerCacheService(HybridCache cache, UserHelper userHelper) : ICustomerCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheCustomerAsync(Customer customer, string company)
    {
        var cacheKey = $"customer-{customer.Id}{userHelper.NormaliseCompanyForCache(company)}";
        var item = new CustomerDto(customer.Id, customer.InstitutionId, customer.FirstName, customer.LastName);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearCustomerAsync(Guid customerId, string company)
    {
        var cacheKey = $"customer-{customerId}{userHelper.NormaliseCompanyForCache(company)}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearCustomersListAsync(string company)
    {
        var cacheKey = $"customers-list{userHelper.NormaliseCompanyForCache(company)}";
        await cache.RemoveAsync(cacheKey);
    }
}
