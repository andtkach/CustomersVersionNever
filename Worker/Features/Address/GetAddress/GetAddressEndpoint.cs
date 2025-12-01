using Common.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Address.GetAddress;

public record AddressResponse(Guid Id, Guid CustomerId, string Country, string City, string Street, bool Current);

internal static class GetAddressEndpoint
{
    public static async Task<IResult> GetAddressAsync(
        [FromRoute] Guid addressId,
        BackendDataContext context,
        ILogger<Program> logger,
        UserHelper userHelper)
    {
        try
        {
            var company = userHelper.GetCompanyHeader();
            var address = await context.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.Company == company);
            
            if (address == null)
            {
                return Results.NotFound();
            }

            var response = new AddressResponse(
                address.Id,
                address.CustomerId,
                address.Country,
                address.City,
                address.Street,
                address.Current);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting address {AddressId}", addressId);
            return Results.Problem("An error occurred while retrieving the address");
        }
    }
}