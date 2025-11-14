using Api.Features.Institution.CreateInstitution;
using Api.Features.Institution.DeleteInstitution;
using Api.Features.Institution.UpdateInstitution;
using Notes.Api.Features.CreateNote;
using Notes.Api.Features.GetNote;
using Notes.Api.Features.GetNotes;

namespace Api.Features;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/notes-sync", CreateNoteEndpoint.CreateNoteSync);
        app.MapPost("/notes-async", CreateNoteEndpoint.CreateNoteAsync);
        app.MapGet("/notes/{id:guid}", GetNoteEndpoint.GetNote);
        app.MapGet("/notes", GetNotesEndpoint.GetNotes);
        
        //institutions
        app.MapPost("/institutions", CreateInstitutionEndpoint.CreateInstitutionAsync);
        app.MapPut("/institutions/{institutionId}", UpdateInstitutionEndpoint.UpdateInstitutionAsync);
        app.MapDelete("/institutions/{institutionId}", DeleteInstitutionEndpoint.DeleteInstitutionAsync);

        return app;
    }
}
