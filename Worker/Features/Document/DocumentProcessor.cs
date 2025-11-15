using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Caching.Hybrid;
using System.Text.Json;
using Common.Contracts;
using Common.Requests.Document;
using Worker.Data;

namespace Worker.Features.Document;

internal sealed class DocumentProcessor(
    ServiceBusClient serviceBusClient,
    IServiceScopeFactory scopeFactory,
    ILogger<DocumentProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        };
        await using var processor = serviceBusClient.CreateProcessor("Documents", options);

        try
        {
            processor.ProcessMessageAsync += ProcessMessage;
            processor.ProcessErrorAsync += args =>
            {
                logger.LogError(args.Exception, "Error while processing document message");
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync(stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        finally
        {
            await processor.StopProcessingAsync(stoppingToken);
        }
    }

    private async Task ProcessMessage(ProcessMessageEventArgs args)
    {
        var mutation = JsonSerializer.Deserialize<EntityMutation>(args.Message.Body);

        if (mutation is null)
        {
            return;
        }

        using var scope = scopeFactory.CreateScope();

        logger.LogInformation("Document mutation {MutationIntentId}", mutation.IntentId);

        var mutationHandler = scope.ServiceProvider.GetRequiredService<IDocumentMutationHandler>();
        var result = await mutationHandler.HandleAsync(
            new DocumentMutationRequest(mutation.IntentId, mutation.Action),
            scope.ServiceProvider.GetRequiredService<FrontendDataContext>(),
            scope.ServiceProvider.GetRequiredService<BackendDataContext>(),
            scope.ServiceProvider.GetRequiredService<HybridCache>(),
            scope.ServiceProvider.GetRequiredService<ILogger<Program>>());

        if (result)
        {
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
