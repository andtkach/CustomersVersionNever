using System.Security.Claims;
using Auth.Data;
using Common.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Auth.Authorization;

public static class Extensions
{
    public static async Task SeedRolesAndPermissions(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var adminRole = await roleManager.FindByNameAsync(Roles.Admin);
        if (adminRole is null)
        {
            adminRole = new IdentityRole(Roles.Admin);
            await roleManager.CreateAsync(adminRole);

            await roleManager.AddClaimAsync(
                adminRole,
                new Claim(CustomClaimTypes.Permission, Permissions.DataRead));

            await roleManager.AddClaimAsync(
                adminRole,
                new Claim(CustomClaimTypes.Permission, Permissions.DataWrite));

            await roleManager.AddClaimAsync(
                adminRole,
                new Claim(CustomClaimTypes.Permission, Permissions.DataRemove));
        }

        var memberRole = await roleManager.FindByNameAsync(Roles.Member);
        if (memberRole is null)
        {
            memberRole = new IdentityRole(Roles.Member);
            await roleManager.CreateAsync(memberRole);

            await roleManager.AddClaimAsync(
                memberRole,
                new Claim(CustomClaimTypes.Permission, Permissions.DataRead));
        }
    }

    public static async Task SeedAdminUser(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // create admin user if it doesn't exist
        var adminUser = await userManager.FindByNameAsync("admin@email.com");

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@email.com", Email = "admin@email.com", Company = "Admin Corp"
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");

            if (result.Succeeded)
            {
                // Assign admin role to the user
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);

                // Add specific permissions for admin user if needed, or rely on role claims
                await userManager.AddClaimAsync(adminUser,
                    new Claim(CustomClaimTypes.Permission, Permissions.DataRead));
                await userManager.AddClaimAsync(adminUser,
                    new Claim(CustomClaimTypes.Permission, Permissions.DataWrite));
                await userManager.AddClaimAsync(adminUser,
                    new Claim(CustomClaimTypes.Permission, Permissions.DataRemove));
            }
        }
    }
}
