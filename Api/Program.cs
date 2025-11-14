using Microsoft.EntityFrameworkCore;
using Notely.ServiceDefaults;
using Api.Data;
using Api.Features;

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

builder.Services.AddHttpClient("TagsApi", client =>
{
    client.BaseAddress = new Uri("https+http://worker");
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Apply EF migrations
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FrontendDataContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.MapNoteEndpoints();

await app.RunAsync();
