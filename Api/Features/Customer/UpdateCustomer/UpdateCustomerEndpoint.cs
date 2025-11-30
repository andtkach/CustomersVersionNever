using Api.Abstractions;
using Api.Data;
using Api.Features.Customer.Services;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Customer.UpdateCustomer
{
    internal static class UpdateCustomerEndpoint
    {
        public record Request(Guid InstitutionId, string FirstName, string LastName);
        
        private static readonly BaseCommandHandler<UpdateCustomerCommand, CustomerUpdatePayload> Handler = new();
        
        public static async Task<IResult> UpdateCustomerAsync(
            [FromBody] Request request,
            [FromRoute] Guid customerId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper,
            [FromServices] ICustomerCacheService cacheService)
        {
            var command = new UpdateCustomerCommand(customerId, request.InstitutionId, request.FirstName, request.LastName);
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
                () => Results.NoContent());
        }
    }
}
