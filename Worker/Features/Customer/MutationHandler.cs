using Common.Model;
using Common.Requests.Customer;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data;

namespace Worker.Features.Customer;

public interface ICustomerMutationHandler
{
    Task<bool> HandleAsync(
        CustomerMutationRequest request,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache,
        ILogger logger);
}

internal sealed class CustomerMutationHandler : ICustomerMutationHandler
{
    private readonly ICustomerOperationFactory _operationFactory;

    public CustomerMutationHandler(ICustomerOperationFactory operationFactory)
    {
        _operationFactory = operationFactory;
    }

    public async Task<bool> HandleAsync(
        CustomerMutationRequest request,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache,
        ILogger logger)
    {
        try
        {
            var intent = await frontendDataContext.Intents.FindAsync(request.IntentId)
                ?? throw new InvalidOperationException($"Unable to find intent with id {request.IntentId}");

            var operation = _operationFactory.CreateOperation(intent.Action);
            await operation.ExecuteAsync(intent, frontendDataContext, backendDataContext, hybridCache);

            intent.State = States.Completed;
            intent.UpdatedAtUtc = DateTime.UtcNow;
            
            await frontendDataContext.SaveChangesAsync();
            await backendDataContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing intent {IntentId}", request.IntentId);
            return false;
        }
    }
}
