namespace Common.Requests.Address;

public record AddressMutationRequest(Guid IntentId, string Action);
