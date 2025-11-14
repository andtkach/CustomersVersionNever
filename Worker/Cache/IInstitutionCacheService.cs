namespace Worker.Cache;

public interface IInstitutionCacheService
{
    Task CacheInstitutionAsync(Data.Institution institution);
    Task ClearInstitutionAsync(Guid institutionId);
    Task ClearInstitutionsListAsync();
}