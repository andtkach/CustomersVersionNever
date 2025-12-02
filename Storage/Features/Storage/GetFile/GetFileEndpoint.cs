using Microsoft.AspNetCore.Mvc;

namespace Storage.Features.Storage.GetFile;

internal static class GetFileEndpoint
{
    public static async Task<IResult> GetFileAsync(
        [FromRoute] Guid fileId,
        [FromServices] IBlobStorageService storage,
        [FromServices] ILogger<Program> logger)
    {
        var result = await storage.GetAsync(fileId).ConfigureAwait(false);
        if (result is null)
        {
            logger.LogInformation("Get file not found: {FileId}", fileId);
            return Results.NotFound();
        }

        var (stream, contentType) = result.Value;
        // Stream will be disposed by the framework after response is sent.
        return Results.File(stream, contentType, fileDownloadName: fileId.ToString("D"));
    }
}