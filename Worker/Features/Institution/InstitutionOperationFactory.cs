using Worker.Cache;
using Worker.Features.Institution.Mutations;

namespace Worker.Features.Institution;

public class InstitutionOperationFactory : IInstitutionOperationFactory
{
    private readonly IInstitutionCacheService _cacheService;
    private readonly Dictionary<string, Func<IInstitutionOperation>> _operationFactories;

    public InstitutionOperationFactory(IInstitutionCacheService cacheService)
    {
        _cacheService = cacheService;
        _operationFactories = new Dictionary<string, Func<IInstitutionOperation>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Create", () => new CreateInstitutionOperation(_cacheService) },
            { "Update", () => new UpdateInstitutionOperation(_cacheService) },
            { "Delete", () => new DeleteInstitutionOperation(_cacheService) },
            { "Patch", () => new PatchInstitutionOperation(_cacheService) },
        };
    }

    public IInstitutionOperation CreateOperation(string action)
    {
        return _operationFactories.TryGetValue(action, out var factory) 
            ? factory() 
            : throw new InvalidOperationException($"Unknown intent action: {action}");
    }
}