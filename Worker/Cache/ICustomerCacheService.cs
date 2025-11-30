using Worker.Data.Model;

namespace Worker.Cache;

public interface ICustomerCacheService
{
    Task CacheCustomerAsync(Customer customer, string company);
    Task ClearCustomerAsync(Guid customerId, string company);
    Task ClearCustomersListAsync(string company);
}