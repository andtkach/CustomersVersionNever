using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Notely.Shared.DTOs;
using Api.Data;

namespace Notes.Api.Features.GetNotes;

internal static class GetNotesEndpoint
{
    public record NoteResponse(
        Guid Id,
        string Title,
        string Content,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        List<TagResponse> Tags); 
    public record GetNotesResponse(List<NoteResponse> Notes);

    public static async Task<IResult> GetNotes(
        FrontendDataContext context,
        HybridCache hybridCache,
        IHttpClientFactory httpClientFactory,
        ILogger<Program> logger)
    {
        try
        {
            var notes = await context.Notes
                .OrderByDescending(n => n.CreatedAtUtc)
                .ToListAsync();

            var noteResponses = new List<NoteResponse>();

            foreach (var note in notes)
            {
                // Get tags for each note
                var tags = await GetTagsForNote(note.Id, hybridCache, httpClientFactory, logger);
                
                noteResponses.Add(new NoteResponse(
                    note.Id,
                    note.Title,
                    note.Content,
                    note.CreatedAtUtc,
                    note.UpdatedAtUtc,
                    tags
                ));
            }

            var response = new GetNotesResponse(noteResponses);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting notes");
            return Results.Problem("An error occurred while retrieving notes");
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
