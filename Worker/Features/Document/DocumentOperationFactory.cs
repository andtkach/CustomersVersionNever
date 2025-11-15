using Worker.Cache;
using Worker.Features.Document.Mutations;

namespace Worker.Features.Document;

public class DocumentOperationFactory : IDocumentOperationFactory
{
    private readonly IDocumentCacheService _cacheService;
    private readonly Dictionary<string, Func<IDocumentOperation>> _operationFactories;

    public DocumentOperationFactory(IDocumentCacheService cacheService)
    {
        _cacheService = cacheService;
        _operationFactories = new Dictionary<string, Func<IDocumentOperation>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Create", () => new CreateDocumentOperation(_cacheService) },
            { "Update", () => new UpdateDocumentOperation(_cacheService) },
            { "Delete", () => new DeleteDocumentOperation(_cacheService) },
            { "Patch", () => new PatchDocumentOperation(_cacheService) },
        };
    }

    public IDocumentOperation CreateOperation(string action)
    {
        return _operationFactories.TryGetValue(action, out var factory) 
            ? factory() 
            : throw new InvalidOperationException($"Unknown intent action: {action}");
    }
}