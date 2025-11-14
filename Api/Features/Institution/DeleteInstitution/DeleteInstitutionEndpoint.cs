using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Features.Institution.Abstractions;

namespace Api.Features.Institution.DeleteInstitution
{
    internal static class DeleteInstitutionEndpoint
    {
        private static readonly InstitutionCommandHandler<DeleteInstitutionCommand, Common.Requests.InstitutionDeletePayload> Handler = new();
        
        public static async Task<IResult> DeleteInstitutionAsync(
            [FromRoute] Guid institutionId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var command = new DeleteInstitutionCommand(institutionId);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                logger,
                () => Results.Ok());
        }
    }
}
