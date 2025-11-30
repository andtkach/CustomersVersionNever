using Api.Abstractions;
using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Customer;
using Microsoft.AspNetCore.Mvc;

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
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            var customerId = Guid.CreateVersion7();
            var command = new CreateCustomerCommand(customerId, request.InstitutionId, request.FirstName, request.LastName);
            var response = new Response(customerId, request.InstitutionId, request.FirstName, request.LastName);
            var company = userHelper.GetUserCompany();
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Customer",
                "Customers",
                logger,
                () => Results.Created($"customers/{customerId}", response));
        }
    }
}
