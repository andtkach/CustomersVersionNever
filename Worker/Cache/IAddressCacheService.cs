using Worker.Data.Model;

namespace Worker.Cache;

public interface IAddressCacheService
{
    Task CacheAddressAsync(Address address, string company);
    Task ClearAddressAsync(Guid addressId, string company);
    Task ClearAddressesListAsync(string company);
}