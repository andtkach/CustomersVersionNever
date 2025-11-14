using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Features.Institution.Abstractions;

namespace Api.Features.Institution.UpdateInstitution
{
    internal static class UpdateInstitutionEndpoint
    {
        public record Request(string Name, string Description);
        
        private static readonly InstitutionCommandHandler<UpdateInstitutionCommand, Common.Requests.InstitutionUpdatePayload> Handler = new();
        
        public static async Task<IResult> UpdateInstitutionAsync(
            [FromBody] Request request,
            [FromRoute] Guid institutionId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var command = new UpdateInstitutionCommand(institutionId, request.Name, request.Description);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                logger,
                () => Results.NoContent());
        }
    }
}
