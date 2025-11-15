using Microsoft.AspNetCore.Mvc;
using Worker.Data;

namespace Worker.Features.Document.GetDocument;

public record DocumentResponse(Guid Id, Guid CustomerId, string Title, string Content, bool Active);

internal static class GetDocumentEndpoint
{
    public static async Task<IResult> GetDocumentAsync(
        [FromRoute] Guid documentId,
        BackendDataContext context,
        ILogger<Program> logger)
    {
        try
        {
            var document = await context.Documents.FindAsync(documentId);
            
            if (document == null)
            {
                return Results.NotFound();
            }

            var response = new DocumentResponse(
                document.Id,
                document.CustomerId,
                document.Title,
                document.Content,
                document.Active);

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting document {DocumentId}", documentId);
            return Results.Problem("An error occurred while retrieving the document");
        }
    }
}