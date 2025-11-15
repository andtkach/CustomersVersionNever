using Microsoft.Extensions.Caching.Hybrid;
using Common.Dto;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class AddressCacheService(HybridCache cache) : IAddressCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheAddressAsync(Address address)
    {
        var cacheKey = $"address-{address.Id}";
        var item = new AddressDto(address.Id, address.CustomerId, address.Country, address.City, address.Street, address.Current);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearAddressAsync(Guid addressId)
    {
        var cacheKey = $"address-{addressId}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearAddressesListAsync()
    {
        var cacheKey = "addresses-list";
        await cache.RemoveAsync(cacheKey);
    }
}
