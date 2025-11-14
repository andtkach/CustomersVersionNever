using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Caching.Hybrid;
using Notely.Shared.Contracts;
using Notely.Shared.DTOs;
using System.Net;
using System.Text.Json;
using Worker.Data;

namespace Tags.Api.Features.AnalyzeNote;

internal sealed class NoteCreatedProcessor(
    ServiceBusClient serviceBusClient,
    IServiceScopeFactory scopeFactory,
    ILogger<NoteCreatedProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false
        };
        await using var processor = serviceBusClient.CreateProcessor("notes", options);

        try
        {
            processor.ProcessMessageAsync += ProcessMessage;
            processor.ProcessErrorAsync += args =>
            {
                logger.LogError(args.Exception, "Error while processing message");
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
        var noteCreated = JsonSerializer.Deserialize<NoteCreated>(args.Message.Body);

        if (noteCreated is null)
        {
            // Log error or throw exception or publish some sort of failure message
            return;
        }

        using var scope = scopeFactory.CreateScope();

        var result = await AnalyzeNoteEndpoint.AnalyzeNote(
            new AnalyzeNoteRequest(noteCreated.NoteId, noteCreated.Title, noteCreated.Content),
            scope.ServiceProvider.GetRequiredService<BackendDataContext>(),
            scope.ServiceProvider.GetRequiredService<HybridCache>(),
            scope.ServiceProvider.GetRequiredService<ILogger<Program>>());

        if (result is IStatusCodeHttpResult { StatusCode: (int)HttpStatusCode.OK })
        {
            await args.CompleteMessageAsync(args.Message);
        }
    }
}
