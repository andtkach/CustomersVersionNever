using Microsoft.AspNetCore.Mvc;
using Api.Features.Institution.Services;

namespace Api.Features.Institution.GetInstitution;

internal static class GetInstitutionEndpoint
{
    public static async Task<IResult> GetInstitutionAsync(
        [FromRoute] Guid id,
        IInstitutionCacheService cacheService,
        ILogger<Program> logger)
    {
        try
        {
            var institution = await cacheService.GetInstitutionAsync(id);
            
            if (institution == null)
            {
                return Results.NotFound($"Institution with id {id} not found");
            }

            return Results.Ok(institution);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institution {InstitutionId}", id);
            return Results.Problem("An error occurred while retrieving the institution");
        }
    }
}