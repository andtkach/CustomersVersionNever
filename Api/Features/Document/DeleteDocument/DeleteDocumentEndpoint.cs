using Api.Abstractions;
using Api.Data;
using Api.Features.Document.Services;
using Azure.Messaging.ServiceBus;
using Common.Authorization;
using Common.Requests.Document;
using Microsoft.AspNetCore.Mvc;

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
            ILogger<Program> logger,
            [FromServices] UserHelper userHelper,
            [FromServices] IDocumentCacheService cacheService)
        {
            var command = new DeleteDocumentCommand(documentId);
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
                () => Results.Ok());
        }
    }
}
