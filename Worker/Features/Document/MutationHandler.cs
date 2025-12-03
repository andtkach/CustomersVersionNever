using Common.Model;
using Common.Requests.Document;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data;
using Worker.Features.Institution;

namespace Worker.Features.Document;

public interface IDocumentMutationHandler
{
    Task<bool> HandleAsync(
        DocumentMutationRequest request,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache,
        ILogger logger,
        string messageId);
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
        ILogger logger,
        string messageId)
    {
        var consumerName = nameof(DocumentMutationHandler);
        var intent = await frontendDataContext.Intents.FindAsync(request.IntentId)
                     ?? throw new InvalidOperationException($"Unable to find intent with id {request.IntentId}");

        if (await frontendDataContext.Consumers.AnyAsync(c =>
                c.MessageId == messageId && c.ConsumerName == consumerName))
        {
            logger.LogWarning($"Message with id {messageId} already was processed by {consumerName}");
            return true;
        }

        var strategy = frontendDataContext.Database.CreateExecutionStrategy();

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                // create transaction under the execution strategy
                await using var transaction = await frontendDataContext.Database.BeginTransactionAsync();

                try
                {
                    var operation = _operationFactory.CreateOperation(intent.Action);
                    await operation.ExecuteAsync(intent, frontendDataContext, backendDataContext, hybridCache);

                    intent.State = States.Completed;
                    intent.UpdatedAtUtc = DateTime.UtcNow;

                    frontendDataContext.Consumers.Add(new Consumer
                    {
                        MessageId = messageId,
                        ConsumerName = consumerName,
                        ConsumedAtUtc = DateTime.UtcNow
                    });

                    await frontendDataContext.SaveChangesAsync();
                    await backendDataContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw; // let the strategy decide whether to retry
                }
            });

            return true;
        }
        catch (Exception ex)
        {
            // This runs after retries are exhausted
            intent.State = States.Failed;
            intent.UpdatedAtUtc = DateTime.UtcNow;
            await frontendDataContext.SaveChangesAsync(); // persist failed intent
            logger.LogError(ex, "Error processing intent {IntentId}", request.IntentId);
            return false;
        }
    }
}
