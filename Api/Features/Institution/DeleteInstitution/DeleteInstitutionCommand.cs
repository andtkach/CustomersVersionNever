using Api.Abstractions;
using Common.Requests.Institution;

namespace Api.Features.Institution.DeleteInstitution;

public sealed class DeleteInstitutionCommand(Guid institutionId) : IBaseCommand<InstitutionDeletePayload>
{
    public string Action => "Delete";
    
    public Guid InstitutionId { get; } = institutionId;

    public InstitutionDeletePayload Payload() => new()
    {
        Id = InstitutionId
    };

    public string GetLogMessage() => "Error deleting institution";
    public string GetErrorMessage() => "An error occurred while deleting the institution";
}