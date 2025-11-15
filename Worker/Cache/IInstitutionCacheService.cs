using Worker.Data.Model;

namespace Worker.Cache;

public interface IInstitutionCacheService
{
    Task CacheInstitutionAsync(Institution institution);
    Task ClearInstitutionAsync(Guid institutionId);
    Task ClearInstitutionsListAsync();
    Task ClearInstitutionListsAsync();
}