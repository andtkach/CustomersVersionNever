using Api.Abstractions;
using Common.Requests.Address;

namespace Api.Features.Address.DeleteAddress;

public sealed class DeleteAddressCommand(Guid addressId) : IBaseCommand<AddressDeletePayload>
{
    public string Action => "Delete";
    
    public Guid AddressId { get; } = addressId;

    public AddressDeletePayload Payload() => new()
    {
        Id = AddressId
    };

    public string GetLogMessage() => "Error deleting address";
    public string GetErrorMessage() => "An error occurred while deleting the address";
}