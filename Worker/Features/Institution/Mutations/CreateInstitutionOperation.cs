using System.Text.Json;
using Common.Model;
using Common.Requests;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Institution.Mutations;

public class CreateInstitutionOperation(IInstitutionCacheService cacheService) : IInstitutionOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<InstitutionCreatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var newInstitution = new Data.Model.Institution
        {
            Id = payload.Id,
            Name = payload.Name,
            Description = payload.Description,
        };

        await backendDataContext.Institutions.AddAsync(newInstitution);
        await cacheService.CacheInstitutionAsync(newInstitution);
        await cacheService.ClearInstitutionsListAsync();
    }
}