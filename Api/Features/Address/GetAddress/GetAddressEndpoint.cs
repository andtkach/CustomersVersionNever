using Microsoft.AspNetCore.Mvc;
using Api.Features.Address.Services;

namespace Api.Features.Address.GetAddress;

internal static class GetAddressEndpoint
{
    public static async Task<IResult> GetAddressAsync(
        [FromRoute] Guid addressId,
        [FromServices] IAddressCacheService cacheService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            var address = await cacheService.GetAddressAsync(addressId);
            
            if (address == null)
            {
                return Results.NotFound($"Address with id {addressId} not found");
            }

            return Results.Ok(address);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting address {AddressId}", addressId);
            return Results.Problem("An error occurred while retrieving the address");
        }
    }
}