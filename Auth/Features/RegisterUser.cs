using Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Auth.Features;

public static class RegisterUser
{
    public record Request(string Email, string Company, string Password, bool EnableNotifications = false);
    public record Response(string id, string Email, string Company);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("register", async (
            Request request,
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager) =>
        {
            // Use the execution strategy to handle retries
            var executionStrategy = dbContext.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync();
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    Company = request.Company,
                    EnableNotifications = request.EnableNotifications
                };
                IdentityResult identityResult = await userManager.CreateAsync(user, request.Password);
                if (!identityResult.Succeeded)
                {
                    return Results.BadRequest(identityResult.Errors);
                }
                IdentityResult addToRoleResult = await userManager.AddToRoleAsync(user, Roles.Member);
                if (!addToRoleResult.Succeeded)
                {
                    return Results.BadRequest(addToRoleResult.Errors);
                }
                await transaction.CommitAsync();
                return Results.Ok(new Response(user.Id, user.Email, user.Company));
            });
        });
    }
}
