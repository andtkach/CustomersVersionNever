using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Notely.Shared.DTOs;
using Worker.Data;

namespace Tags.Api.Features.GetTagsByNote;

internal static class GetTagsByNoteEndpoint
{
    public static async Task<IResult> GetTagsByNote(
        Guid noteId,
        BackendDataContext context,
        HybridCache hybridCache,
        ILogger<Program> logger)
    {
        try
        {
            var cacheKey = $"note_tags_{noteId}";
            var tags = await hybridCache.GetOrCreateAsync(cacheKey,
                async _ =>
                {
                    var tags = await context.Tags
                        .Where(t => t.NoteId == noteId)
                        .OrderBy(t => t.Name)
                        .Select(t => new TagResponse(t.Id, t.Name, t.Color, t.CreatedAtUtc))
                        .ToListAsync();

                    return tags;
                });

            var response = new GetTagsByNoteResponse(tags);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting tags for note {NoteId}", noteId);
            return Results.Problem("An error occurred while retrieving tags");
        }
    }
}
