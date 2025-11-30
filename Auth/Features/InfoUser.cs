using System.Security.Claims;
using Common.Authorization;
using PermissionExtensions = Common.PermissionExtensions;

namespace Auth.Features;

public static class InfoUser
{
    public record Request(string Email, string Password);
    public record Response(string Id, string Token);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("me", (
            ClaimsPrincipal claimsPrincipal,
            IConfiguration configuration) =>
        {
            return Results.Ok(claimsPrincipal.Claims.GroupBy(c => c.Type)
                .ToDictionary(
                    c => c.Key,
                    c => string.Join(',', c.Select(cc => cc.Value))));
        })
        .RequireAuthorization(policy => PermissionExtensions.RequirePermission(policy, Permissions.DataRead));
    }
}
