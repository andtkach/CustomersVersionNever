using Api.Abstractions;
using Common.Requests.Customer;

namespace Api.Features.Customer.CreateCustomer;

public sealed class CreateCustomerCommand(Guid customerId, Guid institutionId, string firstName, string lastName)
    : IBaseCommand<CustomerCreatePayload>
{
    public string Action => "Create";
    
    public Guid CustomerId { get; } = customerId;
    public Guid InstitutionId { get; } = institutionId;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    public CustomerCreatePayload Payload() => new()
    {
        Id = CustomerId,
        InstitutionId = InstitutionId,
        FirstName = FirstName,
        LastName = LastName
    };

    public string GetLogMessage() => "Error creating customer";
    public string GetErrorMessage() => "An error occurred while creating the customer";
}