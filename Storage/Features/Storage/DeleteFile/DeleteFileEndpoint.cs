using Common.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Storage.Features.Storage.DeleteFile
{
    internal static class DeleteFileEndpoint
    {
        public static async Task<IResult> DeleteFileAsync(
            [FromRoute] Guid fileId,
            ILogger<Program> logger,
            [FromServices] IBlobStorageService storage,
            [FromServices] UserHelper userHelper)
        {
            var removed = await storage.DeleteAsync(fileId).ConfigureAwait(false);

            if (!removed)
            {
                logger.LogInformation("Delete file not found: {FileId}", fileId);
                return Results.NotFound();
            }

            logger.LogInformation("Delete file called for {FileId} by {Company}", fileId, userHelper.GetUserCompany());
            return Results.NoContent();
        }
    }
}
