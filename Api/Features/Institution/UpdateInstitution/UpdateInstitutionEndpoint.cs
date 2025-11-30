using Api.Abstractions;
using Api.Data;
using Api.Features.Institution.Services;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Institution;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Institution.UpdateInstitution
{
    internal static class UpdateInstitutionEndpoint
    {
        public record Request(string Name, string Description);
        
        private static readonly BaseCommandHandler<UpdateInstitutionCommand, InstitutionUpdatePayload> Handler = new();
        
        public static async Task<IResult> UpdateInstitutionAsync(
            [FromBody] Request request,
            [FromRoute] Guid institutionId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper,
            [FromServices] IInstitutionCacheService cacheService)
        {
            var command = new UpdateInstitutionCommand(institutionId, request.Name, request.Description);
            var company = userHelper.GetUserCompany();

            await cacheService.Invalidate(institutionId);
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Institution",
                "Institutions",
                logger,
                () => Results.NoContent());
        }
    }
}
