using System.Text.Json;
using Common.Model;
using Common.Requests.Customer;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Customer.Mutations;

public class PatchCustomerOperation(ICustomerCacheService cacheService) : ICustomerOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<CustomerPatchPayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingCustomer = await backendDataContext.Customers.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find customer with id {payload.Id}");

        var existingInstitution = await backendDataContext.Institutions.FindAsync(payload.InstitutionId)
                                  ?? throw new InvalidOperationException($"Unable to find institution with id {payload.Id}");

        existingCustomer.InstitutionId = payload.InstitutionId ?? existingCustomer.InstitutionId;
        existingCustomer.FirstName = payload.FirstName ?? existingCustomer.FirstName;
        existingCustomer.LastName = payload.LastName ?? existingCustomer.LastName;

        await cacheService.CacheCustomerAsync(existingCustomer);
        await cacheService.ClearCustomersListAsync();
    }
}