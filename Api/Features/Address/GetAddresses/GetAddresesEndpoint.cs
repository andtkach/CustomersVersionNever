using Api.Features.Address.Services;
using Common.Dto;

namespace Api.Features.Address.GetAddresses;

internal static class GetAddressesEndpoint
{
    public record GetAddressesResponse(IEnumerable<AddressDto> Addresses);

    public static async Task<IResult> GetAddressesAsync(
        IAddressCacheService cacheService,
        ILogger<Program> logger)
    {
        try
        {
            var addresses = await cacheService.GetAddressesAsync();
            var response = new GetAddressesResponse(addresses);
            
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting addresses list");
            return Results.Problem("An error occurred while retrieving addresses");
        }
    }
}