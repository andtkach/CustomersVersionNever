using System.Security.Claims;
using System.Text;
using Auth.Authorization;
using Auth.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Features;

public static class LoginUser
{
    public record Request(string Email, string Password);
    public record Response(string Id, string Token);

    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("login", async (
            Request request,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            // You probably want to use the options pattern...
            IConfiguration configuration) =>
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null ||
                !await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Results.Unauthorized();
            }

            var roles = await userManager.GetRolesAsync(user);

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var permissions = await (
                    from role in dbContext.Roles
                    join claim in dbContext.RoleClaims on role.Id equals claim.RoleId
                    where roles.Contains(role.Name!) &&
                          claim.ClaimType == CustomClaimTypes.Permission
                    select claim.ClaimValue)
                .Distinct()
                .ToArrayAsync();

            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                ..roles.Select(r => new Claim(ClaimTypes.Role, r)),
                ..permissions.Select(p => new Claim(CustomClaimTypes.Permission, p))
            ];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Audience"]
            };

            var tokenHandler = new JsonWebTokenHandler();

            string accessToken = tokenHandler.CreateToken(tokenDescriptor);

            return Results.Ok(new Response(user.Id, accessToken));
        }); 
    }
}
