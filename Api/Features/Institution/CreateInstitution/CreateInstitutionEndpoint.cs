using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Notely.Shared.Contracts;
using Notely.Shared.DTOs;
using Api.Data;
using Common.Model;
using Common.Requests;

namespace Api.Features.Institution.CreateInstitution
{
    internal static class CreateInstitutionEndpoint
    {
        public record Request(string Name, string Description);
        public record Response(Guid Id, string Name, string Description);
        
        public static async Task<IResult> CreateInstitutionAsync(
            [FromBody] Request request,
            FrontendDataContext dbContext,
            ServiceBusClient serviceBusClient,
            IHttpClientFactory httpClientFactory,
            ILogger<Program> logger)
        {
            try
            {
                var institutionId = Guid.CreateVersion7();

                var payload = new InstitutionCreatePayload()
                {
                    Id = institutionId,
                    Name = request.Name,
                    Description = request.Description,
                };
                
                var intent = new Intent
                {
                    Id = Guid.CreateVersion7(),
                    Action = "Create",
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

                return Results.Created($"institutions/{institutionId}", response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating institution");
                return Results.Problem("An error occurred while creating the institution");
            }
        }
    }
}
