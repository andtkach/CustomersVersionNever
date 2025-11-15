using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Document;

namespace Api.Features.Document.DeleteDocument
{
    internal static class DeleteDocumentEndpoint
    {
        private static readonly BaseCommandHandler<DeleteDocumentCommand, DocumentDeletePayload> Handler = new();
        
        public static async Task<IResult> DeleteDocumentAsync(
            [FromRoute] Guid documentId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var command = new DeleteDocumentCommand(documentId);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Document",
                "Documents",
                logger,
                () => Results.Ok());
        }
    }
}
