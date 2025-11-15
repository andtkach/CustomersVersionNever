using Common.Model;
using Common.Requests.Document;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data;

namespace Worker.Features.Document;

public interface IDocumentMutationHandler
{
    Task<bool> HandleAsync(
        DocumentMutationRequest request,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache,
        ILogger logger);
}

internal sealed class DocumentMutationHandler : IDocumentMutationHandler
{
    private readonly IDocumentOperationFactory _operationFactory;

    public DocumentMutationHandler(IDocumentOperationFactory operationFactory)
    {
        _operationFactory = operationFactory;
    }

    public async Task<bool> HandleAsync(
        DocumentMutationRequest request,
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
