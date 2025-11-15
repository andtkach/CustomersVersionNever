using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Customer;

namespace Api.Features.Customer.CreateCustomer
{
    internal static class CreateCustomerEndpoint
    {
        public record Request(Guid InstitutionId, string FirstName, string LastName);
        public record Response(Guid Id, Guid InstitutionId, string FirstName, string LastName);
        
        private static readonly BaseCommandHandler<CreateCustomerCommand, CustomerCreatePayload> Handler = new();
        
        public static async Task<IResult> CreateCustomerAsync(
            [FromBody] Request request,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var customerId = Guid.CreateVersion7();
            var command = new CreateCustomerCommand(customerId, request.InstitutionId, request.FirstName, request.LastName);
            var response = new Response(customerId, request.InstitutionId, request.FirstName, request.LastName);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Customer",
                "Customers",
                logger,
                () => Results.Created($"customers/{customerId}", response));
        }
    }
}
