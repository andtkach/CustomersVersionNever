using Api.Abstractions;
using Api.Data;
using Api.Features.Address.Services;
using Api.Features.Document.Services;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Document;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;

namespace Api.Features.Document.CreateDocument
{
    internal static class CreateDocumentEndpoint
    {
        public record Request(Guid CustomerId, Guid InstitutionId, string Title, string Content, bool Active);
        public record Response(Guid Id, Guid CustomerId, string Title, string Content, bool Active);
        
        private static readonly BaseCommandHandler<CreateDocumentCommand, DocumentCreatePayload> Handler = new();
        
        public static async Task<IResult> CreateDocumentAsync(
            [FromBody] Request request,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper,
            [FromServices] IDocumentCacheService cacheService)
        {
            var documentId = Guid.CreateVersion7();
            var command = new CreateDocumentCommand(documentId, request.CustomerId, request.Title, request.Content, request.Active);
            var response = new Response(documentId, request.CustomerId, request.Title, request.Content, request.Active);
            var company = userHelper.GetUserCompany();

            await cacheService.Invalidate(documentId);
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
