using System.Text.Json;
using Common.Model;
using Common.Requests.Institution;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Cache;
using Worker.Data;

namespace Worker.Features.Institution.Mutations;

public class DeleteInstitutionOperation(IInstitutionCacheService cacheService) : IInstitutionOperation
{
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

        if (!existingInstitution.Company.Equals(intent.Company))
            throw new InvalidOperationException($"Unable to delete institution with id {payload.Id} by {intent.Company}");

        backendDataContext.Institutions.Remove(existingInstitution);
        await cacheService.ClearInstitutionAsync(existingInstitution.Id, intent.Company);
        await cacheService.ClearInstitutionListsAsync(intent.Company);
    }
}