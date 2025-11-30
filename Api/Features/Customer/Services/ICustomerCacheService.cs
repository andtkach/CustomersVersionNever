using Common.Dto;

namespace Api.Features.Customer.Services;

public interface ICustomerCacheService
{
    Task<CustomerDto?> GetCustomerAsync(Guid id);
    Task<IEnumerable<CustomerDto>> GetCustomersAsync();
    Task Invalidate(Guid id);
}