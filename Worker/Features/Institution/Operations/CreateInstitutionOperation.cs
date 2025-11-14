using Common.Model;
using Common.Requests;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Institution.Operations;

public class CreateInstitutionOperation : IInstitutionOperation
{
    private readonly IInstitutionCacheService _cacheService;

    public CreateInstitutionOperation(IInstitutionCacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache)
    {
        var payload = JsonSerializer.Deserialize<InstitutionCreatePayload>(intent.Payload)
            ?? throw new InvalidOperationException($"Unable to deserialize payload for intent {intent.Id}");

        var newInstitution = new Data.Institution
        {
            Id = payload.Id,
            Name = payload.Name,
            Description = payload.Description,
        };

        await backendDataContext.Institutions.AddAsync(newInstitution);
        await _cacheService.CacheInstitutionAsync(newInstitution);
        await _cacheService.ClearInstitutionsListAsync();
    }
}