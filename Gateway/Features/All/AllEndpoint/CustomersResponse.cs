using Common.Dto;

namespace Gateway.Features.All.AllEndpoint;

public class CustomersResponse
{
    public IEnumerable<CustomerDto> Customers { get; set; } = [];
}