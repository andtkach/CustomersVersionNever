using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Features.Institution.Abstractions;

namespace Api.Features.Institution.PatchInstitution
{
    internal static class PatchInstitutionEndpoint
    {
        public record Request(string? Name, string? Description);
        
        private static readonly InstitutionCommandHandler<PatchInstitutionCommand, Common.Requests.InstitutionPatchPayload> Handler = new();
        
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
                logger,
                () => Results.NoContent());
        }
    }
}
