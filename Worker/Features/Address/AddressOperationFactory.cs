using Worker.Cache;
using Worker.Features.Address.Mutations;

namespace Worker.Features.Address;

public class AddressOperationFactory : IAddressOperationFactory
{
    private readonly IAddressCacheService _cacheService;
    private readonly Dictionary<string, Func<IAddressOperation>> _operationFactories;

    public AddressOperationFactory(IAddressCacheService cacheService)
    {
        _cacheService = cacheService;
        _operationFactories = new Dictionary<string, Func<IAddressOperation>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Create", () => new CreateAddressOperation(_cacheService) },
            { "Update", () => new UpdateAddressOperation(_cacheService) },
            { "Delete", () => new DeleteAddressOperation(_cacheService) },
            { "Patch", () => new PatchAddressOperation(_cacheService) },
        };
    }

    public IAddressOperation CreateOperation(string action)
    {
        return _operationFactories.TryGetValue(action, out var factory) 
            ? factory() 
            : throw new InvalidOperationException($"Unknown intent action: {action}");
    }
}