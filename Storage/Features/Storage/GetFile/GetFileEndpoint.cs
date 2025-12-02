using Microsoft.AspNetCore.Mvc;

namespace Storage.Features.Storage.GetFile;

internal static class GetFileEndpoint
{
    public static async Task<IResult> GetFileAsync(
        [FromRoute] Guid fileId,
        [FromServices] ILogger<Program> logger)
    {
        logger.LogInformation($"Get file called for {fileId}");
        return await Task.FromResult(Results.Ok());
    }
}