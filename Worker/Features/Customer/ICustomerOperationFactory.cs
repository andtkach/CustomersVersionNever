namespace Worker.Features.Customer;

public interface ICustomerOperationFactory
{
    ICustomerOperation CreateOperation(string action);
}