using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Model;
using Common.Requests;
using Microsoft.AspNetCore.Mvc;
using Notely.Shared.Contracts;
using System.Text.Json;

namespace Api.Features.Institution.DeleteInstitution
{
    internal static class DeleteInstitutionEndpoint
    {
        public static async Task<IResult> DeleteInstitutionAsync(
            [FromRoute] Guid institutionId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            try
            {
                var payload = new InstitutionDeletePayload()
                {
                    Id = institutionId,
                };

                var intent = new Intent
                {
                    Id = Guid.CreateVersion7(),
                    Action = "Delete",
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

                return Results.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating institution");
                return Results.Problem("An error occurred while updating the institution");
            }
        }
    }
}
