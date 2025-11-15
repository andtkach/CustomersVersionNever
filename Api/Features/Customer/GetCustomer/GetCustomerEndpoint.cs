using Microsoft.AspNetCore.Mvc;
using Api.Features.Customer.Services;

namespace Api.Features.Customer.GetCustomer;

internal static class GetCustomerEndpoint
{
    public static async Task<IResult> GetCustomerAsync(
        [FromRoute] Guid customerId,
        [FromServices] ICustomerCacheService cacheService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            var customer = await cacheService.GetCustomerAsync(customerId);
            
            if (customer == null)
            {
                return Results.NotFound($"Customer with id {customerId} not found");
            }

            return Results.Ok(customer);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting customer {CustomerId}", customerId);
            return Results.Problem("An error occurred while retrieving the customer");
        }
    }
}