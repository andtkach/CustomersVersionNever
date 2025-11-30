using Api.Abstractions;
using Api.Data;
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
            [FromServices] UserHelper userHelper)
        {
            var command = new DeleteDocumentCommand(documentId);
            var company = userHelper.GetUserCompany();
            
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
