namespace Notely.Shared.Contracts;

public sealed record NoteCreated(Guid NoteId, string Title, string Content);
