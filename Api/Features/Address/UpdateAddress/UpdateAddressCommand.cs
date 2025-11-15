using Api.Abstractions;
using Common.Requests.Address;

namespace Api.Features.Address.UpdateAddress;

public sealed class UpdateAddressCommand(Guid addressId, Guid customerId, Guid institutionId, string country, string city, string street, bool current)
    : IBaseCommand<AddressUpdatePayload>
{
    public string Action => "Update";
    
    public Guid AddressId { get; } = addressId;
    public Guid CustomerId { get; } = customerId;
    public Guid InstitutionId { get; } = institutionId;
    public string Country { get; } = country;
    public string City { get; } = city;
    public string Street { get; } = street;
    public bool Current { get; } = current;

    public AddressUpdatePayload Payload() => new()
    {
        Id = AddressId,
        CustomerId = CustomerId,
        InstitutionId = InstitutionId,
        Country = Country,
        City = City,
        Street = Street,
        Current = Current
    };

    public string GetLogMessage() => "Error updating address";
    public string GetErrorMessage() => "An error occurred while updating the address";
}