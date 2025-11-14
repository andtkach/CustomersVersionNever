using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Institution.GetInstitution;

public record InstitutionResponse(Guid Id, string Name, string Description);

internal static class GetInstitutionEndpoint
{
    public static async Task<IResult> GetInstitutionAsync(
        [FromRoute] Guid id,
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var institution = await context.Institutions.FindAsync(id);
            
            if (institution == null)
            {
                return Results.NotFound();
            }

            var response = new InstitutionResponse(
                institution.Id,
                institution.Name,
                institution.Description);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting institution {InstitutionId}", id);
            return Results.Problem("An error occurred while retrieving the institution");
        }
    }
}