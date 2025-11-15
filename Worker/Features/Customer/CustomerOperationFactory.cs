using Worker.Cache;
using Worker.Features.Customer.Mutations;
using Worker.Features.Institution.Mutations;

namespace Worker.Features.Customer;

public class CustomerOperationFactory : ICustomerOperationFactory
{
    private readonly ICustomerCacheService _cacheService;
    private readonly Dictionary<string, Func<ICustomerOperation>> _operationFactories;

    public CustomerOperationFactory(ICustomerCacheService cacheService)
    {
        _cacheService = cacheService;
        _operationFactories = new Dictionary<string, Func<ICustomerOperation>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Create", () => new CreateCustomerOperation(_cacheService) },
            { "Update", () => new UpdateCustomerOperation(_cacheService) },
            { "Delete", () => new DeleteCustomerOperation(_cacheService) },
            { "Patch", () => new PatchCustomerOperation(_cacheService) },
        };
    }

    public ICustomerOperation CreateOperation(string action)
    {
        return _operationFactories.TryGetValue(action, out var factory) 
            ? factory() 
            : throw new InvalidOperationException($"Unknown intent action: {action}");
    }
}