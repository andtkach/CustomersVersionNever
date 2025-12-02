using Common.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Storage.Features.Storage.UploadFile
{
    internal static class UploadFileEndpoint
    {
        public static async Task<IResult> UploadFileAsync(
            [FromRoute] Guid fileId,
            [FromForm] IFormFile? file,
            ILogger<Program> logger,
            [FromServices] IBlobStorageService storage,
            [FromServices] UserHelper userHelper)
        {
            if (file is null)
            {
                return Results.BadRequest(new { Error = "File is required in form-data." });
            }

            await using var stream = file.OpenReadStream();
            await storage.UploadAsync(fileId, stream, file.ContentType ?? "application/octet-stream").ConfigureAwait(false);

            logger.LogInformation("Upload file called for {FileId} by {Company}", fileId, userHelper.GetUserCompany());
            return Results.Created($"/storage/{fileId}", null);
        }
    }
}
