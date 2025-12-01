using System.Text.Json;
using Common.Model;
using Common.Requests.Address;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Address.Mutations;

public class DeleteAddressOperation(IAddressCacheService cacheService) : IAddressOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<AddressDeletePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingAddress = await backendDataContext.Addresses.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find address with id {payload.Id}");

        if (!existingAddress.Company.Equals(intent.Company))
            throw new InvalidOperationException($"Unable to delete address with id {payload.Id} by {intent.Company}");

        backendDataContext.Addresses.Remove(existingAddress);
        await cacheService.ClearAddressAsync(existingAddress.Id, intent.Company);
        await cacheService.ClearAddressesListAsync(intent.Company);
    }
}