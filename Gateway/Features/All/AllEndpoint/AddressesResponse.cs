using Common.Dto;

namespace Gateway.Features.All.AllEndpoint;

public class AddressesResponse
{
    public IEnumerable<AddressDto> Addresses { get; set; } = [];
}