namespace Common.Requests.Customer;

public record CustomerMutationRequest(Guid IntentId, string Action);
