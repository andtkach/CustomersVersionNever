using System.Text.Json;
using Common.Model;
using Common.Requests;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Institution.Mutations;

public class UpdateInstitutionOperation(IInstitutionCacheService cacheService) : IInstitutionOperation
{
    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<InstitutionUpdatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingInstitution = await backendDataContext.Institutions.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find institution with id {payload.Id}");

        existingInstitution.Name = payload.Name;
        existingInstitution.Description = payload.Description;

        await cacheService.CacheInstitutionAsync(existingInstitution);
        await cacheService.ClearInstitutionsListAsync();
    }
}