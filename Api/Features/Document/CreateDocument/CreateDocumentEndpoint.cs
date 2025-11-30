using Api.Abstractions;
using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Document;
using Microsoft.AspNetCore.Mvc;

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
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper)
        {
            var documentId = Guid.CreateVersion7();
            var command = new CreateDocumentCommand(documentId, request.CustomerId, request.InstitutionId, request.Title, request.Content, request.Active);
            var response = new Response(documentId, request.CustomerId, request.InstitutionId, request.Title, request.Content, request.Active);
            var company = userHelper.GetUserCompany();
            
            return await Handler.ExecuteAsync(
                command,
                dbContext,
                serviceBusClient,
                company,
                "Document",
                "Documents",
                logger,
                () => Results.Created($"customers/{documentId}", response));
        }
    }
}
