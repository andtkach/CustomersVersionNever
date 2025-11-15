using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Model;
using System.Text.Json;
using Common.Contracts;

namespace Api.Features.Institution.Abstractions;

public interface IInstitutionCommand<TPayload>
{
    string Action { get; }
    TPayload CreatePayload();
    string GetLogMessage();
    string GetErrorMessage();
}

public sealed class InstitutionCommandHandler<TCommand, TPayload> 
    where TCommand : IInstitutionCommand<TPayload>
{
    public async Task<IResult> ExecuteAsync(
        TCommand command,
        FrontendDataContext dbContext,
        ServiceBusClient serviceBusClient,
        ILogger logger,
        Func<IResult> onSuccess)
    {
        try
        {
            var payload = command.CreatePayload();
            
            var intent = new Intent
            {
                Id = Guid.CreateVersion7(),
                Action = command.Action,
                Entity = "Institution",
                Payload = JsonSerializer.Serialize(payload),
                State = States.Created,
                CreatedAtUtc = DateTime.UtcNow
            };

            dbContext.Intents.Add(intent);
            await dbContext.SaveChangesAsync();

            await using var sender = serviceBusClient.CreateSender("institutions");
            var mutation = new InstitutionMutation(intent.Id, intent.Action);
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