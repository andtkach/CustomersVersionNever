using Api.Features.Customer.Services;
using Common.Dto;

namespace Api.Features.Customer.GetCustomers;

internal static class GetCustomersEndpoint
{
    public record GetCustomersResponse(IEnumerable<CustomerDto> Customers);

    public static async Task<IResult> GetCustomersAsync(
        ICustomerCacheService cacheService,
        ILogger<Program> logger)
    {
        try
        {
            var customers = await cacheService.GetCustomersAsync();
            var response = new GetCustomersResponse(customers);
            
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting customers list");
            return Results.Problem("An error occurred while retrieving customers");
        }
    }
}