using System.Text.Json;
using Common.Model;
using Common.Requests.Address;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Address.Mutations;

public class PatchAddressOperation(IAddressCacheService cacheService) : IAddressOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<AddressPatchPayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingAddress = await backendDataContext.Addresses.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find address with id {payload.Id}");

        var existingCustomer = await backendDataContext.Customers.FindAsync(payload.CustomerId)
                                  ?? throw new InvalidOperationException($"Unable to find customer with id {payload.Id}");
        
        if (!existingAddress.Company.Equals(intent.Company))
            throw new InvalidOperationException($"Unable to patch address with id {payload.Id} by {intent.Company}");

        existingAddress.CustomerId = payload.CustomerId ?? existingAddress.CustomerId;
        existingAddress.Country = payload.Country ?? existingAddress.Country;
        existingAddress.City = payload.City ?? existingAddress.City;
        existingAddress.Street = payload.Street ?? existingAddress.Street;
        existingAddress.Current = payload.Current ?? existingAddress.Current;

        await cacheService.CacheAddressAsync(existingAddress, intent.Company);
        await cacheService.ClearAddressesListAsync(intent.Company);
    }
}