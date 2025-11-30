using Common.Authorization;
using Microsoft.EntityFrameworkCore;
using ServiceDefaults;
using Worker.Cache;
using Worker.Data;
using Worker.Features;
using Worker.Features.Address;
using Worker.Features.Customer;
using Worker.Features.Document;
using Worker.Features.Institution;

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserHelper>();

builder.Services.AddHostedService<InstitutionProcessor>();
builder.Services.AddHostedService<CustomerProcessor>();
builder.Services.AddHostedService<DocumentProcessor>();
builder.Services.AddHostedService<AddressProcessor>();

builder.Services.AddScoped<IInstitutionCacheService, InstitutionCacheService>();
builder.Services.AddScoped<IInstitutionOperationFactory, InstitutionOperationFactory>();
builder.Services.AddScoped<IInstitutionMutationHandler, InstitutionMutationHandler>();

builder.Services.AddScoped<ICustomerCacheService, CustomerCacheService>();
builder.Services.AddScoped<ICustomerOperationFactory, CustomerOperationFactory>();
builder.Services.AddScoped<ICustomerMutationHandler, CustomerMutationHandler>();

builder.Services.AddScoped<IDocumentCacheService, DocumentCacheService>();
builder.Services.AddScoped<IDocumentOperationFactory, DocumentOperationFactory>();
builder.Services.AddScoped<IDocumentMutationHandler, DocumentMutationHandler>();

builder.Services.AddScoped<IAddressCacheService, AddressCacheService>();
builder.Services.AddScoped<IAddressOperationFactory, AddressOperationFactory>();
builder.Services.AddScoped<IAddressMutationHandler, AddressMutationHandler>();

builder.Services.AddDbContext<BackendDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BackendDb"));
});
builder.EnrichSqlServerDbContext<BackendDataContext>();

builder.Services.AddDbContext<FrontendDataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("FrontendDb"));
});
builder.EnrichSqlServerDbContext<FrontendDataContext>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<BackendDataContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.MapInstitutionsEndpoints();
app.MapCustomersEndpoints();
app.MapDocumentsEndpoints();
app.MapAddressesEndpoints();
await app.RunAsync();
