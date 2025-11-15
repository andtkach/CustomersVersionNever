using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Institution;

namespace Api.Features.Institution.DeleteInstitution
{
    internal static class DeleteInstitutionEndpoint
    {
        private static readonly BaseCommandHandler<DeleteInstitutionCommand, InstitutionDeletePayload> Handler = new();
        
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
                "Institution",
                "Institutions",
                logger,
                () => Results.Ok());
        }
    }
}
