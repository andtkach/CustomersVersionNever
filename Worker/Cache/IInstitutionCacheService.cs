using Worker.Data.Model;

namespace Worker.Cache;

public interface IInstitutionCacheService
{
    Task CacheInstitutionAsync(Institution institution, string company);
    Task ClearInstitutionAsync(Guid institutionId, string company);
    Task ClearInstitutionsListAsync(string company);
    Task ClearInstitutionListsAsync(string company);
}