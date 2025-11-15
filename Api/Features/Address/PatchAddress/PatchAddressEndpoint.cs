using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Address;

namespace Api.Features.Address.PatchAddress
{
    internal static class PatchAddressEndpoint
    {
        public record Request(Guid? CustomerId, Guid? InstitutionId, string? Country, string? City, string? Street, bool? Current);
        
        private static readonly BaseCommandHandler<PatchAddressCommand, AddressPatchPayload> Handler = new();
        
        public static async Task<IResult> PatchAddressAsync(
            [FromBody] Request request,
            [FromRoute] Guid addressId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var command = new PatchAddressCommand(addressId, request.CustomerId, request.InstitutionId, request.Country, request.City, request.Street, request.Current);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Address",
                "Addresses",
                logger,
                () => Results.NoContent());
        }
    }
}
