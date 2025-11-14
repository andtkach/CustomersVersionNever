using Worker.Features.Institution.GetInstitution;
using Worker.Features.Institution.GetInstitutions;

namespace Worker.Features;

public static class WorkerEndpoints
{
    public static IEndpointRouteBuilder MapTagsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("institutions/{id:guid}", GetInstitutionEndpoint.GetInstitutionAsync);
        app.MapGet("institutions", GetInstitutionsEndpoint.GetInstitutionsAsync);

        return app;
    }
}
