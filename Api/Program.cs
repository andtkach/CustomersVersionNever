using Api.Data;
using Api.Features;
using Common.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceDefaults;
using System.Security.Claims;
using System.Text;

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

const string aspireServiceBusName = "ServiceBus";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureServiceBusClient(aspireServiceBusName);

var sqlCacheConn = builder.Configuration.GetConnectionString("CacheDb");
if (string.IsNullOrEmpty(sqlCacheConn))
{
    throw new InvalidOperationException("Cache connection string is not configured.");
}

builder.AddCache(sqlCacheConn);

builder.Services.AddDbContext<FrontendDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FrontendDb"));
});
builder.EnrichSqlServerDbContext<FrontendDataContext>();

builder.Services.AddHttpClient("WorkerApi", client =>
{
    client.BaseAddress = new Uri("http://worker");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("UiPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:20043", "https://localhost:20043")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddScoped<UserHelper>();
builder.Services.AddScoped<Api.Features.Institution.Services.IInstitutionCacheService, Api.Features.Institution.Services.InstitutionCacheService>();
builder.Services.AddScoped<Api.Features.Customer.Services.ICustomerCacheService, Api.Features.Customer.Services.CustomerCacheService>();
builder.Services.AddScoped<Api.Features.Document.Services.IDocumentCacheService, Api.Features.Document.Services.DocumentCacheService>();
builder.Services.AddScoped<Api.Features.Address.Services.IAddressCacheService, Api.Features.Address.Services.AddressCacheService>();

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

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
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("UiPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/ping", (IWebHostEnvironment env) =>
{
    var appName = env.ApplicationName;
    var version = "1.0.0";
    return Results.Ok(new { Service = appName, Version = version, Status = "Healthy" });
});

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FrontendDataContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapInstitutionsEndpoints();
await app.RunAsync();

