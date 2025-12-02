using System.Diagnostics;
using System.IO;

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

var storage = builder.AddProject<Projects.Storage>("Storage")
    .WithHttpsEndpoint(20053, name: "public")
    .WithReference(api)
    .WithReference(worker)
    .WaitFor(api)
    .WaitFor(worker);

Console.WriteLine($"Created {auth}", auth);
Console.WriteLine($"Created {worker}", worker);
Console.WriteLine($"Created {api}", api);
Console.WriteLine($"Created {storage}", storage);

TryEnsureComposeServicesRunning();

await builder.Build().RunAsync();

void TryEnsureComposeServicesRunning()
{
    try
    {
        var composeFile = Path.Combine(Directory.GetCurrentDirectory(), "docker-compose.yml");
        if (!File.Exists(composeFile))
        {
            Console.WriteLine("docker-compose.yml not found in working directory; skipping compose startup.");
            return;
        }

        // Bring up both customers-ui and azurite services defined in docker-compose.yml
        var psi = new ProcessStartInfo
        {
            FileName = "docker",
            Arguments = $"compose -f \"{composeFile}\" up -d customers-ui azurite",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = Process.Start(psi);
        if (proc == null)
        {
            Console.WriteLine("Failed to start docker process for compose; is Docker installed?");
            return;
        }

        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit(30000);

        if (proc.ExitCode == 0)
        {
            Console.WriteLine("Compose services started (customers-ui, azurite).");
            if (!string.IsNullOrWhiteSpace(stdout)) Console.WriteLine(stdout);
        }
        else
        {
            Console.WriteLine($"docker compose exited with code {proc.ExitCode}. Stderr:\n{stderr}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not start compose services: {ex.Message}");
    }
}
