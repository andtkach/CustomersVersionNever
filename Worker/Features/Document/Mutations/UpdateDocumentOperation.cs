using System.Text.Json;
using Common.Model;
using Common.Requests.Document;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Document.Mutations;

public class UpdateDocumentOperation(IDocumentCacheService cacheService) : IDocumentOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<DocumentUpdatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingDocument = await backendDataContext.Documents.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find document with id {payload.Id}");

        var existingCustomer = await backendDataContext.Customers.FindAsync(payload.CustomerId)
                                  ?? throw new InvalidOperationException($"Unable to find customer with id {payload.Id}");
        
        existingDocument.CustomerId = payload.CustomerId;
        existingDocument.Title = payload.Title;
        existingDocument.Content = payload.Content;
        existingDocument.Active = payload.Active;

        await cacheService.CacheDocumentAsync(existingDocument);
        await cacheService.ClearDocumentsListAsync();
    }
}