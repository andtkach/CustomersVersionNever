using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Notely.Shared.DTOs;
using System.Text.Json;
using Worker.Data;

namespace Tags.Api.Features.AnalyzeNote;

internal static class AnalyzeNoteEndpoint
{
    public static async Task<IResult> AnalyzeNote(
        [FromBody] AnalyzeNoteRequest request,
        BackendDataContext context,
        HybridCache hybridCache,
        ILogger<Program> logger)
    {
        try
        {
            // For now, implement simple tag analysis based on content keywords
            // In the future, this could be replaced with LLM analysis
            var tags = AnalyzeContentForTags(request.Title, request.Content);

            // Save tags to database
            var tagEntities = tags.Select(tag => new Tag
            {
                Id = Guid.NewGuid(),
                Name = tag.Name,
                Color = tag.Color,
                NoteId = request.NoteId,
                CreatedAtUtc = DateTime.UtcNow
            }).ToList();

            context.Tags.AddRange(tagEntities);
            await context.SaveChangesAsync();

            // Optionally, cache the tags for the note
            var cacheKey = $"note_tags_{request.NoteId}";
            await hybridCache.SetAsync(
                cacheKey,
                JsonSerializer.SerializeToUtf8Bytes(tags),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(1) });

            var response = new AnalyzeNoteResponse(request.NoteId, tags);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error analyzing note {NoteId}", request.NoteId);
            return Results.Problem("An error occurred while analyzing the note");
        }
    }

    private static List<TagResponse> AnalyzeContentForTags(string title, string content)
    {
        var tags = new List<TagResponse>();
        var allText = $"{title} {content}".ToLowerInvariant();

        // Simple keyword-based tagging (replace with LLM in the future)
        var tagKeywords = new Dictionary<string, (string name, string color)>
        {
            { "work", ("Work", "#3B82F6") },
            { "personal", ("Personal", "#10B981") },
            { "important", ("Important", "#EF4444") },
            { "urgent", ("Urgent", "#F59E0B") },
            { "idea", ("Idea", "#8B5CF6") },
            { "meeting", ("Meeting", "#06B6D4") },
            { "project", ("Project", "#84CC16") },
            { "todo", ("Todo", "#F97316") },
            { "reminder", ("Reminder", "#EC4899") },
            { "note", ("Note", "#6B7280") }
        };

        foreach (var keyword in tagKeywords)
        {
            if (allText.Contains(keyword.Key))
            {
                tags.Add(new TagResponse(
                    Guid.NewGuid(),
                    keyword.Value.name,
                    keyword.Value.color,
                    DateTime.UtcNow
                ));
            }
        }

        // If no tags found, add a default "General" tag
        if (!tags.Any())
        {
            tags.Add(new TagResponse(
                Guid.NewGuid(),
                "General",
                "#6B7280",
                DateTime.UtcNow
            ));
        }

        return tags;
    }
}
