using Common.Model;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data;

namespace Worker.Features.Customer;

public interface ICustomerOperation
{
    Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache);
}