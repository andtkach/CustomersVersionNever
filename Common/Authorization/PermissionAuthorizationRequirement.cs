using Microsoft.AspNetCore.Authorization;

namespace Common.Authorization;

public class PermissionAuthorizationRequirement(params string[] allowedPermissions)
    : AuthorizationHandler<PermissionAuthorizationRequirement>, IAuthorizationRequirement
{
    public string[] AllowedPermissions { get; } = allowedPermissions;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        var permissionClaims = context.User.FindAll(CustomClaimTypes.Permission).ToList();
        
        foreach (var permission in requirement.AllowedPermissions)
        {
            bool found = permissionClaims.Any(c => c.Value == permission);

            if (found)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }
        
        return Task.CompletedTask;
    }
}