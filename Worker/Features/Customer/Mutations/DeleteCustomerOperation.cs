using System.Text.Json;
using Common.Model;
using Common.Requests.Customer;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Customer.Mutations;

public class DeleteCustomerOperation(ICustomerCacheService cacheService) : ICustomerOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<CustomerDeletePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingCustomer = await backendDataContext.Customers.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find customer with id {payload.Id}");

        backendDataContext.Customers.Remove(existingCustomer);
        await cacheService.ClearCustomerAsync(existingCustomer.Id);
        await cacheService.ClearCustomersListAsync();
    }
}