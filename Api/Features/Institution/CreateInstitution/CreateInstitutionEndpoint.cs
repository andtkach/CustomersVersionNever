using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Institution;

namespace Api.Features.Institution.CreateInstitution
{
    internal static class CreateInstitutionEndpoint
    {
        public record Request(string Name, string Description);
        public record Response(Guid Id, string Name, string Description);
        
        private static readonly BaseCommandHandler<CreateInstitutionCommand, InstitutionCreatePayload> Handler = new();
        
        public static async Task<IResult> CreateInstitutionAsync(
            [FromBody] Request request,
            [FromServices] FrontendDataContext dbContext,
            [FromServices] ServiceBusClient serviceBusClient,
            [FromServices] IHttpClientFactory httpClientFactory,
            [FromServices] ILogger<Program> logger)
        {
            var institutionId = Guid.CreateVersion7();
            var command = new CreateInstitutionCommand(institutionId, request.Name, request.Description);
            var response = new Response(institutionId, request.Name, request.Description);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Institution",
                "Institutions",
                logger,
                () => Results.Created($"institutions/{institutionId}", response));
        }
    }
}
