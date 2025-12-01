using System.Text.Json;
using Common.Model;
using Common.Requests.Document;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Document.Mutations;

public class CreateDocumentOperation(IDocumentCacheService cacheService) : IDocumentOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<DocumentCreatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingCustomer = await backendDataContext.Customers.FindAsync(payload.CustomerId)
                                  ?? throw new InvalidOperationException($"Unable to find customer with id {payload.CustomerId}");
        
        var newDocument = new Data.Model.Document
        {
            Id = payload.Id,
            CustomerId = existingCustomer.Id,
            Title = payload.Title,
            Content = payload.Content,
            Active = payload.Active,
            Company = intent.Company,
        };

        await backendDataContext.Documents.AddAsync(newDocument);
        await cacheService.CacheDocumentAsync(newDocument, intent.Company);
        await cacheService.ClearDocumentsListAsync(intent.Company);
    }
}