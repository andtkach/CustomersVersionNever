namespace Common.Dto;

public sealed record AddressDto(Guid Id, Guid CustomerId, string Country, string City, string Street, bool Current);