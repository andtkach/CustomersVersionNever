using Common.Dto;

namespace Api.Features.Institution.Services;

public interface IInstitutionCacheService
{
    Task<InstitutionDto?> GetInstitutionAsync(Guid id);
    Task<IEnumerable<InstitutionDto>> GetInstitutionsAsync();
    Task<InstitutionWithCustomersDto?> GetInstitutionWithCustomersAsync(Guid id);
    Task<IEnumerable<InstitutionWithCustomersDto>> GetInstitutionsWithCustomersAsync();
    Task Invalidate(Guid id);
    Task PutNewInstitution(InstitutionWithCustomersDto institutionWithCustomersDto);
}