using Api.Abstractions;
using Common.Requests.Customer;

namespace Api.Features.Customer.PatchCustomer;

public sealed class PatchCustomerCommand(Guid customerId, Guid? institutionId, string? firstName, string? lastName)
    : IBaseCommand<CustomerPatchPayload>
{
    public string Action => "Patch";
    
    public Guid CustomerId { get; } = customerId;
    public Guid? InstitutionId { get; } = institutionId;
    public string? FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;

    public CustomerPatchPayload Payload() => new()
    {
        Id = CustomerId,
        InstitutionId = InstitutionId,
        FirstName = FirstName,
        LastName = LastName
    };

    public string GetLogMessage() => "Error patching customer";
    public string GetErrorMessage() => "An error occurred while patching the customer";
}