using Common.Dto;

namespace Api.Features.Address.Services;

public interface IAddressCacheService
{
    Task<AddressDto?> GetAddressAsync(Guid id);
    Task<IEnumerable<AddressDto>> GetAddressesAsync();
    Task Invalidate(Guid id);
}