using Common.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Storage.Features.Storage.UploadFile
{
    internal static class UploadFileEndpoint
    {
        public static async Task<IResult> UploadFileAsync(
            [FromRoute] Guid fileId,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            logger.LogInformation($"Upload file called for {fileId}");
            return await Task.FromResult(Results.Ok());
        }
    }
}
