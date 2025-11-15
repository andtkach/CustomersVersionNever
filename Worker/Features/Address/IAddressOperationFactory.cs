namespace Worker.Features.Address;

public interface IAddressOperationFactory
{
    IAddressOperation CreateOperation(string action);
}