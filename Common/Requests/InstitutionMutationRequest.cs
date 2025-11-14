namespace Common.Requests;

public record InstitutionMutationRequest(Guid IntentId, string Action);
