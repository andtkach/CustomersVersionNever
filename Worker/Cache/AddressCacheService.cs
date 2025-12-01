using Common.Authorization;
using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class AddressCacheService(HybridCache cache, UserHelper userHelper) : IAddressCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheAddressAsync(Address address, string company)
    {
        var cacheKey = $"address-{address.Id}{{userHelper.NormaliseCompanyForCache(company)";
        var item = new AddressDto(address.Id, address.CustomerId, address.Country, address.City, address.Street, address.Current);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearAddressAsync(Guid addressId, string company)
    {
        var cacheKey = $"address-{addressId}{{userHelper.NormaliseCompanyForCache(company)";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearAddressesListAsync(string company)
    {
        var cacheKey = $"addresses-list{userHelper.NormaliseCompanyForCache(company)}";
        await cache.RemoveAsync(cacheKey);
    }
}
