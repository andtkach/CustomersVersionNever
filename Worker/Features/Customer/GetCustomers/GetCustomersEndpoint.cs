using Common.Authorization;
using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Customer.GetCustomers;

public record CustomerResponse(Guid Id, Guid InstitutionId, string FirstName, string LastName);

internal static class GetCustomersEndpoint
{
    public static async Task<IResult> GetCustomersAsync(
        BackendDataContext context,
        ILogger<Program> logger,
        UserHelper userHelper)
    {
        try
        {
            var company = userHelper.GetCompanyHeader();
            var customers = await context.Customers.Where(i => i.Company == company)
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