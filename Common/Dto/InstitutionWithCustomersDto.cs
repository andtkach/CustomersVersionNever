namespace Common.Dto;

public sealed record InstitutionWithCustomersDto(Guid Id, string Name, string Description, List<InstitutionCustomerDto> Customers);
public sealed record InstitutionCustomerDto(Guid Id, string FirstName, string LastName);