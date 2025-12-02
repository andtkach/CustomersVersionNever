using System.Security.Claims;
using System.Text;
using Auth.Authorization;
using Auth.Data;
using Auth.Features;
using Common.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb"));
});
builder.EnrichSqlServerDbContext<ApplicationDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
        
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                var authHeader = context.Request.Headers.Authorization.ToString();
                logger.LogWarning("Authentication challenge. Authorization header: {AuthHeader}", 
                    string.IsNullOrEmpty(authHeader) ? "(empty)" : authHeader.Substring(0, Math.Min(50, authHeader.Length)));
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                var authHeader = context.Request.Headers.Authorization.ToString();
                logger.LogError(context.Exception, 
                    "Authentication failed. Authorization header: {AuthHeader}. Error: {Error}", 
                    string.IsNullOrEmpty(authHeader) ? "(empty)" : authHeader.Substring(0, Math.Min(100, authHeader.Length)),
                    context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                
                if (context.Principal!.Identity is ClaimsIdentity identity)
                {
                    var permissionClaim = identity.FindFirst("permission");
                    if (permissionClaim != null && permissionClaim.Value.StartsWith('['))
                    {
                        // If permission is stored as JSON array, parse it
                        try
                        {
                            var permissions = System.Text.Json.JsonSerializer.Deserialize<string[]>(permissionClaim.Value);
                            if (permissions != null)
                            {
                                identity.RemoveClaim(permissionClaim);
                                foreach (var perm in permissions)
                                {
                                    identity.AddClaim(new Claim(CustomClaimTypes.Permission, perm));
                                }
                            }
                        }
                        catch
                        {
                            // If parsing fails, leave as is
                        }
                    }
                }
                
                logger.LogInformation("Token validated. Claims: {Claims}", 
                    string.Join(", ", context.Principal!.Claims.Select(c => $"{c.Type}={c.Value}")));
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("UiPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:20043", "https://localhost:20043")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrations();
    await app.SeedRolesAndPermissions();
    await app.SeedAdminUser();
}

app.UseHttpsRedirection();
app.UseCors("UiPolicy");
app.UseAuthentication();
app.UseAuthorization();

InfoUser.MapEndpoint(app);
RegisterUser.MapEndpoint(app);
LoginUser.MapEndpoint(app);

await app.RunAsync();

