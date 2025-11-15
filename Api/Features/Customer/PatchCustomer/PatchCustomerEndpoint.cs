using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Customer;

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
            ILogger<Program> logger)
        {
            var command = new PatchCustomerCommand(customerId, request.InstitutionId, request.FirstName, request.LastName);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Customer",
                "Customers",
                logger,
                () => Results.NoContent());
        }
    }
}
