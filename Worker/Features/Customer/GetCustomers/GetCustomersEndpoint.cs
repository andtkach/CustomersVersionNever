using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.customer.GetCustomers;

public record CustomerResponse(Guid Id, Guid InstitutionId, string FirstName, string LastName);

internal static class GetCustomersEndpoint
{
    public static async Task<IResult> GetCustomersAsync(
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var customers = await context.Customers
                .OrderBy(i => i.FirstName)
                .Select(i => new CustomerResponse(i.Id, i.InstitutionId, i.FirstName, i.LastName))
                .ToListAsync();

            return Results.Ok(customers);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting customers list");
            return Results.Problem("An error occurred while retrieving customers");
        }
    }
}