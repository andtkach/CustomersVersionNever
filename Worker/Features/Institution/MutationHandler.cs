using Azure.Core;
using Common.Model;
using Common.Requests;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;
using Worker.Data;

namespace Worker.Features.Institution;

internal static class MutationHandler
{
    public static async Task<bool> Handle(
        InstitutionMutationRequest request,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache,
        ILogger<Program> logger)
    {
        try
        {

            var intent = await frontendDataContext.Intents.FindAsync(request.IntentId);

            if (intent == null)
            {
                throw new InvalidOperationException($"Unable to find intent with id {request.IntentId}");
            }

            if (intent.Action == "Create")
            {
                var payload = JsonSerializer.Deserialize<InstitutionCreatePayload>(intent.Payload);
                
                if (payload == null)
                {
                    throw new InvalidOperationException($"Unable to deserialize payload for intent {request.IntentId}");
                }

                var newInstitution = new Data.Institution
                {
                    Id = payload.Id,
                    Name = payload.Name,
                    Description = payload.Description,
                };

                await backendDataContext.Institutions.AddAsync(newInstitution);
                await CacheInstitution(newInstitution, hybridCache);
            }
            else if (intent.Action == "Update")
            {
                var payload = JsonSerializer.Deserialize<InstitutionUpdatePayload>(intent.Payload);
                
                if (payload == null)
                {
                    throw new InvalidOperationException($"Unable to deserialize payload for intent {request.IntentId}");
                }

                var existingInstitution = await backendDataContext.Institutions.FindAsync(payload.Id);
                if (existingInstitution == null)
                {
                    throw new InvalidOperationException($"Unable to find institution with id {intent.Id}");
                }

                existingInstitution.Name = payload.Name;
                existingInstitution.Description = payload.Description;
                
                await CacheInstitution(existingInstitution, hybridCache);
            }
            else if (intent.Action == "Delete")
            {
                var payload = JsonSerializer.Deserialize<InstitutionDeletePayload>(intent.Payload);

                if (payload == null)
                {
                    throw new InvalidOperationException($"Unable to deserialize payload for intent {request.IntentId}");
                }

                var existingInstitution = await backendDataContext.Institutions.FindAsync(payload.Id);
                if (existingInstitution == null)
                {
                    throw new InvalidOperationException($"Unable to find institution with id {intent.Id}");
                }

                backendDataContext.Institutions.Remove(existingInstitution);
                
                await ClearInstitution(existingInstitution.Id, hybridCache);
            }
            else
            {
                throw new InvalidOperationException($"Unknown intent action {intent.Action}");
            }


            intent.State = States.Completed;
            intent.UpdatedAtUtc = DateTime.UtcNow;
            await frontendDataContext.SaveChangesAsync();
            
            await backendDataContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing intent {IntentId}", request.IntentId);
            return false;
        }
    }

    private static async Task CacheInstitution(Data.Institution institution, HybridCache cache)
    {
        var cacheKey = $"institution-{institution.Id}";
        await cache.SetAsync(
            cacheKey,
            JsonSerializer.SerializeToUtf8Bytes(institution),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(1) });
    }

    private static async Task ClearInstitution(Guid institutionId, HybridCache cache)
    {
        var cacheKey = $"institution-{institutionId}";
        await cache.RemoveAsync(cacheKey);
    }
}
