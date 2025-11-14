using Tags.Api.Features.AnalyzeNote;
using Tags.Api.Features.GetTagsByNote;

namespace Worker.Features;

public static class WorkerEndpoints
{
    public static IEndpointRouteBuilder MapTagsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("tags/analyze", AnalyzeNoteEndpoint.AnalyzeNote);
        app.MapGet("tags/note/{noteId:guid}", GetTagsByNoteEndpoint.GetTagsByNote);

        return app;
    }
}
