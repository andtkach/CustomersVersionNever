namespace Common.Dto;

public sealed record CustomerDto(Guid Id, Guid InstitutionId, string FirstName, string LastName);