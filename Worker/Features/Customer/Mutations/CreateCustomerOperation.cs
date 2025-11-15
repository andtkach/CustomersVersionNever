using System.Text.Json;
using Common.Model;
using Common.Requests.Customer;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Customer.Mutations;

public class CreateCustomerOperation(ICustomerCacheService cacheService) : ICustomerOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<CustomerCreatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingInstitution = await backendDataContext.Institutions.FindAsync(payload.InstitutionId)
                                  ?? throw new InvalidOperationException($"Unable to find institution with id {payload.InstitutionId}");
        
        var newCustomer = new Data.Model.Customer
        {
            Id = payload.Id,
            InstitutionId = existingInstitution.Id,
            FirstName = payload.FirstName,
            LastName = payload.LastName,
        };

        await backendDataContext.Customers.AddAsync(newCustomer);
        await cacheService.CacheCustomerAsync(newCustomer);
        await cacheService.ClearCustomersListAsync();
    }
}