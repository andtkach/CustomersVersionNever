using Common.Model;
using Common.Requests;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Institution.Operations;

public class UpdateInstitutionOperation : IInstitutionOperation
{
    private readonly IInstitutionCacheService _cacheService;

    public UpdateInstitutionOperation(IInstitutionCacheService cacheService)
    {
        _cacheService = cacheService;
    }

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

        await _cacheService.CacheInstitutionAsync(existingInstitution);
        await _cacheService.ClearInstitutionsListAsync();
    }
}