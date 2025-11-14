using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Features.Institution.Abstractions;

namespace Api.Features.Institution.CreateInstitution
{
    internal static class CreateInstitutionEndpoint
    {
        public record Request(string Name, string Description);
        public record Response(Guid Id, string Name, string Description);
        
        private static readonly InstitutionCommandHandler<CreateInstitutionCommand, Common.Requests.InstitutionCreatePayload> Handler = new();
        
        public static async Task<IResult> CreateInstitutionAsync(
            [FromBody] Request request,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var institutionId = Guid.CreateVersion7();
            var command = new CreateInstitutionCommand(institutionId, request.Name, request.Description);
            var response = new Response(institutionId, request.Name, request.Description);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                logger,
                () => Results.Created($"institutions/{institutionId}", response));
        }
    }
}
