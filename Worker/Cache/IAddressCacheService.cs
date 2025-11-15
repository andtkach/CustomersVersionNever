using Worker.Data.Model;

namespace Worker.Cache;

public interface IAddressCacheService
{
    Task CacheAddressAsync(Address address);
    Task ClearAddressAsync(Guid addressId);
    Task ClearAddressesListAsync();
}