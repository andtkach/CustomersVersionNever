using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Address;

namespace Api.Features.Address.DeleteAddress
{
    internal static class DeleteAddressEndpoint
    {
        private static readonly BaseCommandHandler<DeleteAddressCommand, AddressDeletePayload> Handler = new();
        
        public static async Task<IResult> DeleteAddressAsync(
            [FromRoute] Guid addressId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var command = new DeleteAddressCommand(addressId);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Address",
                "Addresses",
                logger,
                () => Results.Ok());
        }
    }
}
