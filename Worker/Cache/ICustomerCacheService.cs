using Worker.Data.Model;

namespace Worker.Cache;

public interface ICustomerCacheService
{
    Task CacheCustomerAsync(Customer customer);
    Task ClearCustomerAsync(Guid customerId);
    Task ClearCustomersListAsync();
}