using Api.Features.Institution.Abstractions;
using Common.Requests;

namespace Api.Features.Institution.DeleteInstitution;

public sealed class DeleteInstitutionCommand(Guid institutionId) : IInstitutionCommand<InstitutionDeletePayload>
{
    public string Action => "Delete";
    
    public Guid InstitutionId { get; } = institutionId;

    public InstitutionDeletePayload CreatePayload() => new()
    {
        Id = InstitutionId
    };

    public string GetLogMessage() => "Error deleting institution";
    public string GetErrorMessage() => "An error occurred while deleting the institution";
}