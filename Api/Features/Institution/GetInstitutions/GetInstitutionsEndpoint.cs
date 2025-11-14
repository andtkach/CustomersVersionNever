using Api.Features.Institution.Services;
using Common.DTOs;

namespace Api.Features.Institution.GetInstitutions;

internal static class GetInstitutionsEndpoint
{
    public record GetInstitutionsResponse(IEnumerable<InstitutionDto> Institutions);

    public static async Task<IResult> GetInstitutionsAsync(
        IInstitutionCacheService cacheService,
        ILogger<Program> logger)
    {
        try
        {
            var institutions = await cacheService.GetInstitutionsAsync();
            var response = new GetInstitutionsResponse(institutions);
            
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institutions list");
            return Results.Problem("An error occurred while retrieving institutions");
        }
    }
}