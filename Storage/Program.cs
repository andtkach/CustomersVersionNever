using Common.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiceDefaults;
using System.Security.Claims;
using System.Text;
using Storage.Features;
using Azure.Storage;
using Azure.Storage.Blobs;
using Storage.Health;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

var storageConnection = builder.Configuration["AzureStorage:ConnectionString"]
                        ?? builder.Configuration["ConnectionStrings:AzureStorage"]
                        ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

if (!string.IsNullOrWhiteSpace(storageConnection))
{
    try
    {
        builder.Services.AddSingleton(new BlobServiceClient(storageConnection));
    }
    catch (FormatException ex)
    {
        throw new InvalidOperationException(
            "Azure Storage connection string is invalid. Ensure 'AzureStorage:ConnectionString', 'ConnectionStrings:AzureStorage' or the 'AZURE_STORAGE_CONNECTION_STRING' environment variable contains a valid value.",
            ex);
    }
}
else if (builder.Environment.IsDevelopment())
{
    var azuriteBlobEndpoint = builder.Configuration["Azurite:BlobEndpoint"] ?? "http://127.0.0.1:10000/devstoreaccount1";
    var azuriteAccountName = builder.Configuration["Azurite:AccountName"] ?? "devstoreaccount1";
    var azuriteAccountKey = builder.Configuration["Azurite:AccountKey"] ?? Environment.GetEnvironmentVariable("AZURITE_ACCOUNT_KEY");

    if (string.IsNullOrWhiteSpace(azuriteAccountKey))
    {
        throw new InvalidOperationException(
            "No Azure Storage connection configured. For local development set 'Azurite:AccountKey' in configuration or 'AZURITE_ACCOUNT_KEY' environment variable. Do not store secrets in source control.");
    }

    var endpointUri = new Uri(azuriteBlobEndpoint);
    var credential = new StorageSharedKeyCredential(azuriteAccountName, azuriteAccountKey);
    var blobServiceClient = new BlobServiceClient(endpointUri, credential);
    builder.Services.AddSingleton(blobServiceClient);
}
else
{
    throw new InvalidOperationException(
        "Azure Storage configuration missing. Set 'AzureStorage:ConnectionString' or the 'AZURE_STORAGE_CONNECTION_STRING' environment variable for non-development environments.");
}

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<UserHelper>();

builder.Services.AddHealthChecks()
    .AddCheck<BlobStorageHealthCheck>("blob_storage");

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("UiPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/ping", (IWebHostEnvironment env) =>
{
    var appName = env.ApplicationName;
    return Results.Ok(new { Service = appName });
});

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.MapHealthChecks("/health");
app.MapStorageEndpoints();

await app.RunAsync();

