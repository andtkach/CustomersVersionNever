using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Institution;

namespace Api.Features.Institution.PatchInstitution
{
    internal static class PatchInstitutionEndpoint
    {
        public record Request(string? Name, string? Description);
        
        private static readonly BaseCommandHandler<PatchInstitutionCommand, InstitutionPatchPayload> Handler = new();
        
        public static async Task<IResult> PatchInstitutionAsync(
            [FromBody] Request request,
            [FromRoute] Guid institutionId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var command = new PatchInstitutionCommand(institutionId, request.Name, request.Description);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Institution",
                "Institutions",
                logger,
                () => Results.NoContent());
        }
    }
}
