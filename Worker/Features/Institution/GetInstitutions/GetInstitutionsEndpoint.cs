using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worker.Data;
using Worker.Data.Model;

namespace Worker.Features.Institution.GetInstitutions;

public record InstitutionResponse(Guid Id, string Name, string Description);
public record InstitutionWithCustomersResponse(Guid Id, string Name, string Description, List<CustomerResponse> Customers);
public record CustomerResponse(Guid Id, string FirstName, string LastName);

internal static class GetInstitutionsEndpoint
{
    public static async Task<IResult> GetInstitutionsAsync(
        [FromQuery] bool? includeCustomers,
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var shouldIncludeCustomers = includeCustomers ?? false;

            return shouldIncludeCustomers
                ? Results.Ok(await context.Institutions
                    .Include(i => i.Customers)
                    .OrderBy(i => i.Name)
                    .Select(i => 
                        new InstitutionWithCustomersResponse(
                            i.Id, 
                            i.Name, 
                            i.Description, 
                            i.Customers.Select(c => 
                                new CustomerResponse(c.Id, c.FirstName, c.LastName)).ToList()))
                    .ToListAsync())
                : Results.Ok(await context.Institutions
                    .OrderBy(i => i.Name)
                    .Select(i => 
                        new InstitutionResponse(
                            i.Id, 
                            i.Name, 
                            i.Description))
                    .ToListAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institutions list");
            return Results.Problem("An error occurred while retrieving institutions");
        }
    }
}