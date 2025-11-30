using Api.Abstractions;
using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Customer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Customer.PatchCustomer
{
    internal static class PatchCustomerEndpoint
    {
        public record Request(Guid? InstitutionId, string? FirstName, string? LastName);
        
        private static readonly BaseCommandHandler<PatchCustomerCommand, CustomerPatchPayload> Handler = new();
        
        public static async Task<IResult> PatchCustomerAsync(
            [FromBody] Request request,
            [FromRoute] Guid customerId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            var command = new PatchCustomerCommand(customerId, request.InstitutionId, request.FirstName, request.LastName);
            var company = userHelper.GetUserCompany();
            
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
