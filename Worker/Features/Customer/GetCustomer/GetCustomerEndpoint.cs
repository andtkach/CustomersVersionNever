using Microsoft.AspNetCore.Mvc;
using Worker.Data;

namespace Worker.Features.Customer.GetCustomer;

public record CustomerResponse(Guid Id, Guid InstitutionId, string FirstName, string LastName);

internal static class GetCustomerEndpoint
{
    public static async Task<IResult> GetCustomerAsync(
        [FromRoute] Guid customerId,
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var customer = await context.Customers.FindAsync(customerId);
            
            if (customer == null)
            {
                return Results.NotFound();
            }

            var response = new CustomerResponse(
                customer.Id,
                customer.InstitutionId,
                customer.FirstName,
                customer.LastName);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting customer {CustomerId}", customerId);
            return Results.Problem("An error occurred while retrieving the customer");
        }
    }
}