using Common.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Storage.Features.Storage.DeleteFile
{
    internal static class DeleteFileEndpoint
    {
        public static async Task<IResult> DeleteFileAsync(
            [FromRoute] Guid fileId,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            logger.LogInformation($"Delete file called for {fileId}");
            return await Task.FromResult(Results.Ok());
        }
    }
}
