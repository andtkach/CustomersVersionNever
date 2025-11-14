using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Distributed;
using Notely.Shared.DTOs;
using Api.Data;

namespace Notes.Api.Features.GetNote;

internal static class GetNoteEndpoint
{
    public record NoteResponse(
        Guid Id,
        string Title,
        string Content,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        List<TagResponse> Tags);
    
    public static async Task<IResult> GetNote(
        Guid id,
        FrontendDataContext context,
        HybridCache hybridCache,
        IHttpClientFactory httpClientFactory,
        ILogger<Program> logger)
    {
        try
        {
            var note = await context.Notes
                .FirstOrDefaultAsync(n => n.Id == id);

            if (note == null)
            {
                return Results.NotFound($"Note with ID {id} not found");
            }

            // Get tags from Tags API
            var tags = await GetTagsForNote(id, hybridCache, httpClientFactory, logger);

            var response = new NoteResponse(
                note.Id,
                note.Title,
                note.Content,
                note.CreatedAtUtc,
                note.UpdatedAtUtc,
                tags
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting note {NoteId}", id);
            return Results.Problem("An error occurred while retrieving the note");
        }
    }

    private static async Task<List<TagResponse>> GetTagsForNote(
        Guid noteId,
        HybridCache hybridCache,
        IHttpClientFactory httpClientFactory,
        ILogger logger)
    {
        return await hybridCache.GetOrCreateAsync($"note_tags_{noteId}", async _ =>
        {
            try
            {
                var httpClient = httpClientFactory.CreateClient("TagsApi");
                var response = await httpClient.GetAsync($"tags/note/{noteId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GetTagsByNoteResponse>();
                    return result?.Tags ?? [];
                }

                logger.LogWarning(
                    "Failed to get tags for note {NoteId}. Status: {StatusCode}",
                    noteId,
                    response.StatusCode);
                return [];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calling Tags API for note {NoteId}", noteId);
                return [];
            }
        });
    }
}
