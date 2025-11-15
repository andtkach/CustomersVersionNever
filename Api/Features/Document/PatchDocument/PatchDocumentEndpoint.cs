using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Document;

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
            ILogger<Program> logger)
        {
            var command = new PatchDocumentCommand(documentId, request.CustomerId, request.InstitutionId, request.Title, request.Content, request.Active);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Document",
                "Documents",
                logger,
                () => Results.NoContent());
        }
    }
}
