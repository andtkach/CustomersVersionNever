using Api.Features.Institution.CreateInstitution;
using Api.Features.Institution.DeleteInstitution;
using Api.Features.Institution.UpdateInstitution;
using Api.Features.Institution.GetInstitution;
using Api.Features.Institution.GetInstitutions;

namespace Api.Features;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/institutions", CreateInstitutionEndpoint.CreateInstitutionAsync);
        app.MapPut("/institutions/{institutionId}", UpdateInstitutionEndpoint.UpdateInstitutionAsync);
        app.MapDelete("/institutions/{institutionId}", DeleteInstitutionEndpoint.DeleteInstitutionAsync);
        app.MapGet("/institutions/{id:guid}", GetInstitutionEndpoint.GetInstitutionAsync);
        app.MapGet("/institutions", GetInstitutionsEndpoint.GetInstitutionsAsync);
        
        return app;
    }
}
