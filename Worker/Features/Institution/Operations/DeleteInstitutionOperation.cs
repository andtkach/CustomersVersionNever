using Common.Model;
using Common.Requests;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Institution.Operations;

public class DeleteInstitutionOperation : IInstitutionOperation
{
    private readonly IInstitutionCacheService _cacheService;

    public DeleteInstitutionOperation(IInstitutionCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<InstitutionDeletePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var existingInstitution = await backendDataContext.Institutions.FindAsync(payload.Id)
            ?? throw new InvalidOperationException($"Unable to find institution with id {payload.Id}");

        backendDataContext.Institutions.Remove(existingInstitution);
        await _cacheService.ClearInstitutionAsync(existingInstitution.Id);
        await _cacheService.ClearInstitutionsListAsync();
    }
}