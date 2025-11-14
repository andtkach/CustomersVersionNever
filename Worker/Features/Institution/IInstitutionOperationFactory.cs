namespace Worker.Features.Institution;

public interface IInstitutionOperationFactory
{
    IInstitutionOperation CreateOperation(string action);
}