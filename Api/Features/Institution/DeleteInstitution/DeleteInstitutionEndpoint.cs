using Api.Abstractions;
using Api.Data;
using Api.Features.Institution.Services;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Institution;
using Microsoft.AspNetCore.Mvc;

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
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper,
            [FromServices] IInstitutionCacheService cacheService)
        {
            var command = new DeleteInstitutionCommand(institutionId);
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
                () => Results.Ok());
        }
    }
}
