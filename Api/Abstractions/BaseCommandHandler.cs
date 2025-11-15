using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Model;
using System.Text.Json;
using Common.Contracts;

namespace Api.Abstractions;

public interface IBaseCommand<TPayload>
{
    string Action { get; }
    TPayload Payload();
    string GetLogMessage();
    string GetErrorMessage();
}

public sealed class BaseCommandHandler<TCommand, TPayload> 
    where TCommand : IBaseCommand<TPayload>
{
    public async Task<IResult> ExecuteAsync(
        TCommand command,
        FrontendDataContext dbContext,
        ServiceBusClient serviceBusClient,
        string entityName,
        string queueName,
        ILogger logger,
        Func<IResult> onSuccess)
    {
        try
        {
            var payload = command.Payload();
            
            var intent = new Intent
            {
                Id = Guid.CreateVersion7(),
                Action = command.Action,
                Entity = entityName,
                Payload = JsonSerializer.Serialize(payload),
                State = States.Created,
                CreatedAtUtc = DateTime.UtcNow
            };

            dbContext.Intents.Add(intent);
            await dbContext.SaveChangesAsync();

            await using var sender = serviceBusClient.CreateSender(queueName);
            var mutation = new EntityMutation(intent.Id, intent.Action);
            await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(mutation)));

            return onSuccess();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, command.GetLogMessage());
            return Results.Problem(command.GetErrorMessage());
        }
    }
}