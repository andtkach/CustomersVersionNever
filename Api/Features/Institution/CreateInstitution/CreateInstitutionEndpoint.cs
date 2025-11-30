using Api.Abstractions;
using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Institution;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            [FromServices] ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            var institutionId = Guid.CreateVersion7();
            var command = new CreateInstitutionCommand(institutionId, request.Name, request.Description);
            var response = new Response(institutionId, request.Name, request.Description);
            var company = userHelper.GetUserCompany();

            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Institution",
                "Institutions",
                logger,
                () => Results.Created($"institutions/{institutionId}", response));
        }
    }
}
