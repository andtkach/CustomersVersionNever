namespace Common.Requests.Document;

public record DocumentMutationRequest(Guid IntentId, string Action);
