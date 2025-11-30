const string aspireServiceBusName = "ServiceBus";
const string institutionsQueueName = "Institutions";
const string customersQueueName = "Customers";
const string documentsQueueName = "Documents";
const string addressesQueueName = "Addresses";

const string aspireDatabaseServer = "DatabaseServer";
const string aspireCacheDatabase = "CacheDb";
const string aspireFrontendDatabase = "FrontendDb";
const string aspireBackendDatabase = "BackendDb";
const string aspireAuthDatabase = "AuthDb";

var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus(aspireServiceBusName)
    .RunAsEmulator(c => c.WithLifetime(ContainerLifetime.Persistent));

serviceBus.AddServiceBusQueue(institutionsQueueName);
serviceBus.AddServiceBusQueue(customersQueueName);
serviceBus.AddServiceBusQueue(documentsQueueName);
serviceBus.AddServiceBusQueue(addressesQueueName);

var sqlPassword = builder.AddParameter("sql-password", secret: true, value: "sql-password-2025!?");
var database = builder.AddSqlServer(aspireDatabaseServer, port: 1439, password: sqlPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var cacheDatabase = database.AddDatabase(aspireCacheDatabase);
var frontendDatabase = database.AddDatabase(aspireFrontendDatabase);
var backendDatabase = database.AddDatabase(aspireBackendDatabase);
var authDatabase = database.AddDatabase(aspireAuthDatabase);

var auth = builder.AddProject<Projects.Auth>("Auth")
    .WithHttpsEndpoint(20033, name: "public")
    .WithReference(authDatabase)
    .WaitFor(authDatabase);


var worker = builder.AddProject<Projects.Worker>("Worker")
    .WithHttpsEndpoint(20023, name: "public")
    .WithReference(backendDatabase)
    .WithReference(serviceBus)
    .WithReference(cacheDatabase)
    .WithReference(frontendDatabase)
    .WithReference(authDatabase)
    .WaitFor(backendDatabase)
    .WaitFor(serviceBus)
    .WaitFor(cacheDatabase)
    .WaitFor(frontendDatabase)
    .WaitFor(authDatabase);

var api = builder.AddProject<Projects.Api>("Api")
    .WithHttpsEndpoint(20013, name: "public")
    .WithReference(frontendDatabase)
    .WithReference(serviceBus)
    .WithReference(worker)
    .WithReference(cacheDatabase)
    .WithReference(authDatabase)
    .WaitFor(frontendDatabase)
    .WaitFor(serviceBus)
    .WaitFor(worker)
    .WaitFor(cacheDatabase)
    .WaitFor(authDatabase);

Console.WriteLine($"Created {auth}", auth);
Console.WriteLine($"Created {worker}", worker);
Console.WriteLine($"Created {api}", api);

await builder.Build().RunAsync();
