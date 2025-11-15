using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Address;

namespace Api.Features.Address.CreateAddress
{
    internal static class CreateAddressEndpoint
    {
        public record Request(Guid CustomerId, string Country, string City, string Street, bool Current);
        public record Response(Guid Id, Guid CustomerId, string Country, string City, string Street, bool Current);
        
        private static readonly BaseCommandHandler<CreateAddressCommand, AddressCreatePayload> Handler = new();
        
        public static async Task<IResult> CreateAddressAsync(
            [FromBody] Request request,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var addressId = Guid.CreateVersion7();
            var command = new CreateAddressCommand(addressId, request.CustomerId, request.Country, request.City, request.Street, request.Current);
            var response = new Response(addressId, request.CustomerId, request.Country, request.City, request.Street, request.Current);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Address",
                "Addresses",
                logger,
                () => Results.Created($"addresses/{addressId}", response));
        }
    }
}
