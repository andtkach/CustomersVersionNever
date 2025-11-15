using Api.Abstractions;
using Common.Requests.Customer;

namespace Api.Features.Customer.DeleteCustomer;

public sealed class DeleteCustomerCommand(Guid customerId) : IBaseCommand<CustomerDeletePayload>
{
    public string Action => "Delete";
    
    public Guid CustomerId { get; } = customerId;

    public CustomerDeletePayload Payload() => new()
    {
        Id = CustomerId
    };

    public string GetLogMessage() => "Error deleting customer";
    public string GetErrorMessage() => "An error occurred while deleting the customer";
}