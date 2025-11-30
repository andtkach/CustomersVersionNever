using Common.Authorization;
using Common.Dto;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data.Model;

namespace Worker.Cache;

public sealed class InstitutionCacheService(HybridCache cache, UserHelper userHelper) : IInstitutionCacheService
{
    private static readonly HybridCacheEntryOptions CacheOptions = new() 
    { 
        Expiration = TimeSpan.FromMinutes(1) 
    };

    public async Task CacheInstitutionAsync(Institution institution, string company)
    {
        var cacheKey = $"institution-{institution.Id}{userHelper.NormaliseCompanyForCache(company)}";
        var item = new InstitutionDto(institution.Id, institution.Name, institution.Description);
        await cache.SetAsync(
            cacheKey,
            item,
            CacheOptions);
    }

    public async Task ClearInstitutionAsync(Guid institutionId, string company)
    {
        var cacheKey = $"institution-{institutionId}{userHelper.NormaliseCompanyForCache(company)}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearInstitutionsListAsync(string company)
    {
        var cacheKey = $"institutions-list{userHelper.NormaliseCompanyForCache(company)}";
        await cache.RemoveAsync(cacheKey);
    }

    public async Task ClearInstitutionListsAsync(string company)
    {
        await cache.RemoveAsync($"institutions-list{userHelper.NormaliseCompanyForCache(company)}");
        await cache.RemoveAsync($"institutions-list-with-customers{userHelper.NormaliseCompanyForCache(company)}");
    }
}
