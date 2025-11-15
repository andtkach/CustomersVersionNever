using Api.Abstractions;
using Common.Requests.Address;

namespace Api.Features.Address.CreateAddress;

public sealed class CreateAddressCommand(Guid addressId, Guid customerId, string country, string city, string street, bool current)
    : IBaseCommand<AddressCreatePayload>
{
    public string Action => "Create";
    
    public Guid AddressId { get; } = addressId;
    public Guid CustomerId { get; } = customerId;
    public string Country { get; } = country;
    public string City { get; } = city;
    public string Street { get; } = street;
    public bool Current { get; } = current;

    public AddressCreatePayload Payload() => new()
    {
        Id = AddressId,
        CustomerId = CustomerId,
        Country = Country,
        City = City,
        Street = Street,
        Current = Current
    };

    public string GetLogMessage() => "Error creating address";
    public string GetErrorMessage() => "An error occurred while creating the address";
}