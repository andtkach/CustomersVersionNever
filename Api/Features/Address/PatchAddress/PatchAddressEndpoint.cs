using Api.Abstractions;
using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Address;
using Microsoft.AspNetCore.Mvc;

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
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            var command = new PatchAddressCommand(addressId, request.CustomerId, request.InstitutionId, request.Country, request.City, request.Street, request.Current);
            var company = userHelper.GetUserCompany();
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Address",
                "Addresses",
                logger,
                () => Results.NoContent());
        }
    }
}
