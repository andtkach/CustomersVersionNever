using System.Text.Json;
using Common.Model;
using Common.Requests.Address;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Address.Mutations;

public class CreateAddressOperation(IAddressCacheService cacheService) : IAddressOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<AddressCreatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingCustomer = await backendDataContext.Customers.FindAsync(payload.CustomerId)
                                  ?? throw new InvalidOperationException($"Unable to find customer with id {payload.CustomerId}");
        
        var newAddress = new Data.Model.Address
        {
            Id = payload.Id,
            CustomerId = existingCustomer.Id,
            Country = payload.Country,
            City = payload.City,
            Street = payload.Street,
            Current = payload.Current,
            Company = intent.Company,
        };

        await backendDataContext.Addresses.AddAsync(newAddress);
        await cacheService.CacheAddressAsync(newAddress, intent.Company);
        await cacheService.ClearAddressesListAsync(intent.Company);
    }
}