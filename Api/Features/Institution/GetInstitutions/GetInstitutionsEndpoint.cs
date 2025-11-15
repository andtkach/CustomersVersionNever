using Api.Features.Institution.Services;
using Common.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Institution.GetInstitutions;

internal static class GetInstitutionsEndpoint
{
    public record GetInstitutionsResponse(IEnumerable<InstitutionDto> Institutions);
    public record GetInstitutionsWithCustomersResponse(IEnumerable<InstitutionWithCustomersDto> Institutions);

    public static async Task<IResult> GetInstitutionsAsync(
        IInstitutionCacheService cacheService,
        [FromQuery] bool? includeCustomers,
        ILogger<Program> logger)
    {
        try
        {
            if (includeCustomers is true)
            {
                return Results.Ok(new GetInstitutionsWithCustomersResponse(await cacheService.GetInstitutionsWithCustomersAsync()));
            }
            else
            {
                return Results.Ok(new GetInstitutionsResponse(await cacheService.GetInstitutionsAsync()));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institutions list");
            return Results.Problem("An error occurred while retrieving institutions");
        }
    }
}