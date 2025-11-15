using Common.DTOs;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class InstitutionCacheService(HybridCache cache) : IInstitutionCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheInstitutionAsync(Institution institution)
    {
        var cacheKey = $"institution-{institution.Id}";
        var item = new InstitutionDto(institution.Id, institution.Name, institution.Description);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearInstitutionAsync(Guid institutionId)
    {
        var cacheKey = $"institution-{institutionId}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearInstitutionsListAsync()
    {
        var cacheKey = "institutions-list";
        await cache.RemoveAsync(cacheKey);
    }
}
