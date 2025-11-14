using Api.Data;
using Azure.Messaging.ServiceBus;
using Common.Model;
using Common.Requests;
using Microsoft.AspNetCore.Mvc;
using Notely.Shared.Contracts;
using Notely.Shared.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Features.Institution.UpdateInstitution
{
    internal static class UpdateInstitutionEndpoint
    {
        public record Request(string Name, string Description);
        public record Response(Guid Id, string Name, string Description);
        
        public static async Task<IResult> UpdateInstitutionAsync(
            [FromBody] Request request,
            [FromRoute] Guid institutionId,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            try
            {
                var payload = new InstitutionUpdatePayload()
                {
                    Id = institutionId,
                    Name = request.Name,
                    Description = request.Description,
                };

                var intent = new Intent
                {
                    Id = Guid.CreateVersion7(),
                    Action = "Update",
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
                var response = new Response(institutionId, request.Name, request.Description);

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating institution");
                return Results.Problem("An error occurred while updating the institution");
            }
        }
    }
}
