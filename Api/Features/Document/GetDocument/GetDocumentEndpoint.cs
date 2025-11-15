using Microsoft.AspNetCore.Mvc;
using Api.Features.Document.Services;

namespace Api.Features.Document.GetDocument;

internal static class GetDocumentEndpoint
{
    public static async Task<IResult> GetDocumentAsync(
        [FromRoute] Guid documentId,
        [FromServices] IDocumentCacheService cacheService,
        [FromServices] ILogger<Program> logger)
    {
        try
        {
            var document = await cacheService.GetDocumentAsync(documentId);
            
            if (document == null)
            {
                return Results.NotFound($"Document with id {documentId} not found");
            }

            return Results.Ok(document);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting customer {DocumentId}", documentId);
            return Results.Problem("An error occurred while retrieving the document");
        }
    }
}