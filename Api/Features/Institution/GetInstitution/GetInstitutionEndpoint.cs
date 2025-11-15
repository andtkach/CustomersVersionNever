using Microsoft.AspNetCore.Mvc;
using Api.Features.Institution.Services;

namespace Api.Features.Institution.GetInstitution;

internal static class GetInstitutionEndpoint
{
    public static async Task<IResult> GetInstitutionAsync(
        [FromRoute] Guid institutionId,
        [FromQuery] bool? includeCustomers,
        [FromServices] IInstitutionCacheService cacheService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            if (includeCustomers is true)
            {
                var institution = await cacheService.GetInstitutionWithCustomersAsync(institutionId);
                return institution == null ? Results.NotFound($"Institution with id {institutionId} not found") : Results.Ok(institution);
            }
            else
            {
                var institution = await cacheService.GetInstitutionAsync(institutionId);
                return institution == null ? Results.NotFound($"Institution with id {institutionId} not found") : Results.Ok(institution);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institution {InstitutionId}", institutionId);
            return Results.Problem("An error occurred while retrieving the institution");
        }
    }
}