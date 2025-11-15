using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Features;
using ServiceDefaults;

AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

const string aspireServiceBusName = "ServiceBus";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureServiceBusClient(aspireServiceBusName);

var sqlCacheConn = builder.Configuration.GetConnectionString("Cache");
if (string.IsNullOrEmpty(sqlCacheConn))
{
    throw new InvalidOperationException("Cache connection string is not configured.");
}

builder.AddCache(sqlCacheConn);

builder.Services.AddDbContext<FrontendDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Frontend"));
});
builder.EnrichSqlServerDbContext<FrontendDataContext>();

builder.Services.AddHttpClient("WorkerApi", client =>
{
    client.BaseAddress = new Uri("http://worker");
});

builder.Services.AddScoped<Api.Features.Institution.Services.IInstitutionCacheService, Api.Features.Institution.Services.InstitutionCacheService>();
builder.Services.AddScoped<Api.Features.Customer.Services.ICustomerCacheService, Api.Features.Customer.Services.CustomerCacheService>();
builder.Services.AddScoped<Api.Features.Document.Services.IDocumentCacheService, Api.Features.Document.Services.DocumentCacheService>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FrontendDataContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.MapInstitutionsEndpoints();
await app.RunAsync();
