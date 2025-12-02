using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Storage.Health
{
    public sealed class BlobStorageHealthCheck : IHealthCheck
    {
        private readonly BlobServiceClient _client;

        public BlobStorageHealthCheck(BlobServiceClient client)
        {
            _client = client;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Perform a lightweight service call: retrieve account properties
                await _client.GetPropertiesAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                return HealthCheckResult.Healthy("Azure Blob Storage reachable.");
            }
            catch (RequestFailedException ex)
            {
                return HealthCheckResult.Unhealthy("Azure Blob Storage returned an error.", ex);
            }
            catch (System.Exception ex)
            {
                return HealthCheckResult.Unhealthy("Azure Blob Storage health check failed.", ex);
            }
        }
    }
}
