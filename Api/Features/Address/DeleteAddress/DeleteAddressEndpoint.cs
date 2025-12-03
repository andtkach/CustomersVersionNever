using Api.Abstractions;
using Api.Data;
using Api.Features.Address.Services;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Address;
using Microsoft.AspNetCore.Mvc;

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
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper,
            [FromServices] IAddressCacheService cacheService)
        {
            var command = new DeleteAddressCommand(addressId);
            var company = userHelper.GetUserCompany();

            await cacheService.Invalidate(addressId);
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Address",
                "Addresses",
                logger,
                () => Results.Ok());
        }
    }
}
