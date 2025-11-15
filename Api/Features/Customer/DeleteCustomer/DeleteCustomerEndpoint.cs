using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Customer;

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
            ILogger<Program> logger)
        {
            var command = new DeleteCustomerCommand(customerId);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Customer",
                "Customers",
                logger,
                () => Results.Ok());
        }
    }
}
