using Common.Authorization;
using Gateway.Features.All.AllEndpoint;

namespace Gateway.Features;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapGatewayEndpoints(this IEndpointRouteBuilder app)
    {
        
        app.MapGet("/all", AllInstitutionEndpoint.GetAllAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));
        
        return app;
    }
}
