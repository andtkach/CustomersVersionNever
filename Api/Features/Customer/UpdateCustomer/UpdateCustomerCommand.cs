using Api.Abstractions;
using Common.Requests.Customer;

namespace Api.Features.Customer.UpdateCustomer;

public sealed class UpdateCustomerCommand(Guid customerId, Guid institutionId, string firstName, string lastName)
    : IBaseCommand<CustomerUpdatePayload>
{
    public string Action => "Update";
    
    public Guid CustomerId { get; } = customerId;
    public Guid InstitutionId { get; } = institutionId;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    public CustomerUpdatePayload Payload() => new()
    {
        Id = CustomerId,
        InstitutionId = InstitutionId,
        FirstName = FirstName,
        LastName = LastName
    };

    public string GetLogMessage() => "Error updating customer";
    public string GetErrorMessage() => "An error occurred while updating the customer";
}