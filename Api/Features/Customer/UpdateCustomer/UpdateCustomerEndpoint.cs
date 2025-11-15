using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Customer;

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
            ILogger<Program> logger)
        {
            var command = new UpdateCustomerCommand(customerId, request.InstitutionId, request.FirstName, request.LastName);
            
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
