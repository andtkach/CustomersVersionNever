using Common.Dto;

namespace Api.Features.Document.Services;

public interface IDocumentCacheService
{
    Task<DocumentDto?> GetDocumentAsync(Guid id);
    Task<IEnumerable<DocumentDto>> GetDocumentsAsync();
    Task Invalidate(Guid id);
}