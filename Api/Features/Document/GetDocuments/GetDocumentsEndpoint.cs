using Api.Features.Document.Services;
using Common.Dto;

namespace Api.Features.Document.GetDocuments;

internal static class GetDocumentsEndpoint
{
    public record GetDocumentsResponse(IEnumerable<DocumentDto> Documents);

    public static async Task<IResult> GetDocumentsAsync(
        IDocumentCacheService cacheService,
        ILogger<Program> logger)
    {
        try
        {
            var documents = await cacheService.GetDocumentsAsync();
            var response = new GetDocumentsResponse(documents);
            
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting documents list");
            return Results.Problem("An error occurred while retrieving documents");
        }
    }
}