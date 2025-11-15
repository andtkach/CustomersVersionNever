using System.Text.Json;
using Common.Model;
using Common.Requests.Address;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Address.Mutations;

public class UpdateAddressOperation(IAddressCacheService cacheService) : IAddressOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<AddressUpdatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingAddress = await backendDataContext.Addresses.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find document with id {payload.Id}");

        var existingCustomer = await backendDataContext.Customers.FindAsync(payload.CustomerId)
                                  ?? throw new InvalidOperationException($"Unable to find customer with id {payload.Id}");
        
        existingAddress.CustomerId = payload.CustomerId;
        existingAddress.Country = payload.Country;
        existingAddress.City = payload.City;
        existingAddress.Street = payload.Street;
        existingAddress.Current = payload.Current;

        await cacheService.CacheAddressAsync(existingAddress);
        await cacheService.ClearAddressesListAsync();
    }
}