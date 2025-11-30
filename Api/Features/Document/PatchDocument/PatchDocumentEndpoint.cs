using Api.Abstractions;
using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Document;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Document.PatchDocument
{
    internal static class PatchDocumentEndpoint
    {
        public record Request(Guid? CustomerId, Guid? InstitutionId, string? Title, string? Content, bool? Active);
        
        private static readonly BaseCommandHandler<PatchDocumentCommand, DocumentPatchPayload> Handler = new();
        
        public static async Task<IResult> PatchDocumentAsync(
            [FromBody] Request request,
            [FromRoute] Guid documentId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            var command = new PatchDocumentCommand(documentId, request.CustomerId, request.InstitutionId, request.Title, request.Content, request.Active);
            var company = userHelper.GetUserCompany();
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Document",
                "Documents",
                logger,
                () => Results.NoContent());
        }
    }
}
