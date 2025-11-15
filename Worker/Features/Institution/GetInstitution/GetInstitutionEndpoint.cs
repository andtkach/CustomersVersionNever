using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Institution.GetInstitution;

public record InstitutionResponse(Guid Id, string Name, string Description);
public record InstitutionWithCustomersResponse(Guid Id, string Name, string Description, List<CustomerResponse> Customers);
public record CustomerResponse(Guid Id, string FirstName, string LastName);

internal static class GetInstitutionEndpoint
{
    public static async Task<IResult> GetInstitutionAsync(
        [FromRoute] Guid institutionId,
        [FromQuery] bool? includeCustomers,
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            bool shouldIncludeCustomers = includeCustomers ?? false;

            var institution = !shouldIncludeCustomers
                ? await context.Institutions.FindAsync(institutionId)
                : await context.Institutions
                    .Include(i => i.Customers)
                    .FirstOrDefaultAsync(i => i.Id == institutionId);

            if (institution == null)
            {
                return Results.NotFound();
            }

            if (shouldIncludeCustomers)
            {
                var customers = institution.Customers.Select(c => new CustomerResponse(c.Id, c.FirstName, c.LastName))
                    .ToList();
                var responseWithCustomers = new InstitutionWithCustomersResponse(
                    institution.Id, 
                    institution.Name,
                    institution.Description, 
                    customers);

                return Results.Ok(responseWithCustomers);
            }
            else
            {
                var response = new InstitutionResponse(
                    institution.Id,
                    institution.Name,
                    institution.Description);

                return Results.Ok(response);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institution {InstitutionId}", institutionId);
            return Results.Problem("An error occurred while retrieving the institution");
        }
    }
}