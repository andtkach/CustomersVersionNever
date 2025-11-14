using Aspire.Hosting;
using Projects;
using System.Diagnostics;

const string aspireServiceBusName = "ServiceBus";
const string institutionsQueueName = "Institutions";
const string aspireDatabaseServer = "Database";
const string aspireCacheDatabase = "Cache";
const string aspireFrontendDatabase = "Frontend";
const string aspireBackendDatabase = "Backend";

var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus(aspireServiceBusName)
    .RunAsEmulator(c => c.WithLifetime(ContainerLifetime.Persistent));

serviceBus.AddServiceBusQueue("notes");
serviceBus.AddServiceBusQueue(institutionsQueueName);

var sqlPassword = builder.AddParameter("sql-password", secret: true, value: "sql-password-2025!?");
var database = builder.AddSqlServer(aspireDatabaseServer, port: 1439, password: sqlPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var cacheDatabase = database.AddDatabase(aspireCacheDatabase);
var frontendDatabase = database.AddDatabase(aspireFrontendDatabase);
var backendDatabase = database.AddDatabase(aspireBackendDatabase);

var worker = builder.AddProject<Projects.Worker>("Worker")
    .WithHttpsEndpoint(5002, name: "public")
    .WithReference(backendDatabase)
    .WithReference(serviceBus)
    .WithReference(cacheDatabase)
    .WithReference(frontendDatabase)
    .WaitFor(backendDatabase)
    .WaitFor(serviceBus)
    .WaitFor(cacheDatabase)
    .WaitFor(frontendDatabase);

var api = builder.AddProject<Projects.Api>("Api")
    .WithHttpsEndpoint(5001, name: "public")
    .WithReference(frontendDatabase)
    .WithReference(serviceBus)
    .WithReference(worker)
    .WithReference(cacheDatabase)
    .WaitFor(frontendDatabase)
    .WaitFor(serviceBus)
    .WaitFor(worker)
    .WaitFor(cacheDatabase);

Console.WriteLine($"Created {worker}", worker);
Console.WriteLine($"Created {api}", api);

await builder.Build().RunAsync();
