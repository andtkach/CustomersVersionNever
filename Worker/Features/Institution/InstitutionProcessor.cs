using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Caching.Hybrid;
using Notely.Shared.Contracts;
using Notely.Shared.DTOs;
using System.Net;
using System.Text.Json;
using Common.Requests;
using Worker.Data;

namespace Worker.Features.Institution;

internal sealed class InstitutionProcessor(
    ServiceBusClient serviceBusClient,
    IServiceScopeFactory scopeFactory,
    ILogger<InstitutionProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        };
        await using var processor = serviceBusClient.CreateProcessor("Institutions", options);

        try
        {
            processor.ProcessMessageAsync += ProcessMessage;
            processor.ProcessErrorAsync += args =>
            {
                logger.LogError(args.Exception, "Error while processing institution message");
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
        var mutation = JsonSerializer.Deserialize<InstitutionMutation>(args.Message.Body);

        if (mutation is null)
        {
            return;
        }

        using var scope = scopeFactory.CreateScope();

        logger.LogInformation("Institution mutation {MutationIntentId}", mutation.IntentId);

        var mutationHandler = scope.ServiceProvider.GetRequiredService<IInstitutionMutationHandler>();
        var result = await mutationHandler.HandleAsync(
            new InstitutionMutationRequest(mutation.IntentId, mutation.Action),
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
