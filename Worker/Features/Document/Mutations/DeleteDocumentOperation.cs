using System.Text.Json;
using Common.Model;
using Common.Requests.Document;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Document.Mutations;

public class DeleteDocumentOperation(IDocumentCacheService cacheService) : IDocumentOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<DocumentDeletePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingDocument = await backendDataContext.Documents.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find document with id {payload.Id}");

        if (!existingDocument.Company.Equals(intent.Company))
            throw new InvalidOperationException($"Unable to delete document with id {payload.Id} by {intent.Company}");

        backendDataContext.Documents.Remove(existingDocument);
        await cacheService.ClearDocumentAsync(existingDocument.Id, intent.Company);
        await cacheService.ClearDocumentsListAsync(intent.Company);
    }
}