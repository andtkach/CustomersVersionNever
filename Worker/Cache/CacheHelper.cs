using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;

namespace Worker.Cache
{
    public static class CacheHelper
    {
        public static async Task CacheInstitution(Data.Institution institution, HybridCache cache)
        {
            var cacheKey = $"institution-{institution.Id}";
            await cache.SetAsync(
                cacheKey,
                JsonSerializer.SerializeToUtf8Bytes(institution),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(1) });
        }

        public static async Task ClearInstitution(Guid institutionId, HybridCache cache)
        {
            var cacheKey = $"institution-{institutionId}";
            await cache.RemoveAsync(cacheKey);
        }
    }
}
