using Common.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Customer.GetCustomer;

public record CustomerResponse(Guid Id, Guid InstitutionId, string FirstName, string LastName);

internal static class GetCustomerEndpoint
{
    public static async Task<IResult> GetCustomerAsync(
        [FromRoute] Guid customerId,
        BackendDataContext context,
        ILogger<Program> logger,
        UserHelper userHelper)
    {
        try
        {
            var company = userHelper.GetCompanyHeader();
            var customer = await context.Customers.FirstOrDefaultAsync(i => i.Id == customerId && i.Company == company);

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