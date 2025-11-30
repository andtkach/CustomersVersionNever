namespace Common.Contracts;

public sealed record EntityMutation(Guid IntentId, string Company, string Action);
