using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Api.Data;
using Api.Abstractions;
using Common.Requests.Document;

namespace Api.Features.Document.CreateDocument
{
    internal static class CreateDocumentEndpoint
    {
        public record Request(Guid CustomerId, Guid InstitutionId, string Title, string Content, bool Active);
        public record Response(Guid Id, Guid CustomerId, Guid InstitutionId, string Title, string Content, bool Active);
        
        private static readonly BaseCommandHandler<CreateDocumentCommand, DocumentCreatePayload> Handler = new();
        
        public static async Task<IResult> CreateDocumentAsync(
            [FromBody] Request request,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            var documentId = Guid.CreateVersion7();
            var command = new CreateDocumentCommand(documentId, request.CustomerId, request.InstitutionId, request.Title, request.Content, request.Active);
            var response = new Response(documentId, request.CustomerId, request.InstitutionId, request.Title, request.Content, request.Active);
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                "Document",
                "Documents",
                logger,
                () => Results.Created($"customers/{documentId}", response));
        }
    }
}
