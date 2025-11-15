using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Address.GetAddresses;

public record AddressResponse(Guid Id, Guid CustomerId, string Country, string City, string Street, bool Current);

internal static class GetAddressesEndpoint
{
    public static async Task<IResult> GetAddressesAsync(
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var addresses = await context.Addresses
                .OrderBy(i => i.Country)
                .Select(d => new AddressResponse(d.Id, d.CustomerId, d.Country, d.City, d.Street, d.Current))
                .ToListAsync();

            return Results.Ok(addresses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting addresses list");
            return Results.Problem("An error occurred while retrieving addresses");
        }
    }
}