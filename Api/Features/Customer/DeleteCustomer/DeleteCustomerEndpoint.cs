using Api.Abstractions;
using Api.Data;
using Api.Features.Customer.Services;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Customer.DeleteCustomer
{
    internal static class DeleteCustomerEndpoint
    {
        private static readonly BaseCommandHandler<DeleteCustomerCommand, CustomerDeletePayload> Handler = new();
        
        public static async Task<IResult> DeleteCustomerAsync(
            [FromRoute] Guid customerId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper,
            [FromServices] ICustomerCacheService cacheService)
        {
            var command = new DeleteCustomerCommand(customerId);
            var company = userHelper.GetUserCompany();

            await cacheService.Invalidate(customerId);
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Customer",
                "Customers",
                logger,
                () => Results.Ok());
        }
    }
}
