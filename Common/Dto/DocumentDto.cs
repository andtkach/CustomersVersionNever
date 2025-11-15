namespace Common.Dto;

public sealed record DocumentDto(Guid Id, Guid CustomerId, string Title, string Content, bool Active);