using Microsoft.EntityFrameworkCore;
using Worker.Data;

namespace Worker.Features.Document.GetDocuments;

public record DocumentResponse(Guid Id, Guid CustomerId, string Title, string Content, bool Active);

internal static class GetDocumentsEndpoint
{
    public static async Task<IResult> GetDocumentsAsync(
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var documents = await context.Documents
                .OrderBy(i => i.Title)
                .Select(d => new DocumentResponse(d.Id, d.CustomerId, d.Title, d.Content, d.Active))
                .ToListAsync();

            return Results.Ok(documents);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting documents list");
            return Results.Problem("An error occurred while retrieving documents");
        }
    }
}