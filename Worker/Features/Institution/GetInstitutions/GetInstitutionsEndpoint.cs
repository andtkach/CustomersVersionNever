using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Institution.GetInstitutions;

public record InstitutionResponse(Guid Id, string Name, string Description);

internal static class GetInstitutionsEndpoint
{
    public static async Task<IResult> GetInstitutionsAsync(
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var institutions = await context.Institutions
                .OrderBy(i => i.Name)
                .Select(i => new InstitutionResponse(i.Id, i.Name, i.Description))
                .ToListAsync();

            return Results.Ok(institutions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institutions list");
            return Results.Problem("An error occurred while retrieving institutions");
        }
    }
}