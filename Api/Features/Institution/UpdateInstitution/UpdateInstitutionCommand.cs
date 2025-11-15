using Api.Abstractions;
using Common.Requests.Institution;

namespace Api.Features.Institution.UpdateInstitution;

public sealed class UpdateInstitutionCommand(Guid institutionId, string name, string description)
    : IBaseCommand<InstitutionUpdatePayload>
{
    public string Action => "Update";
    
    public Guid InstitutionId { get; } = institutionId;
    public string Name { get; } = name;
    public string Description { get; } = description;

    public InstitutionUpdatePayload Payload() => new()
    {
        Id = InstitutionId,
        Name = Name,
        Description = Description
    };

    public string GetLogMessage() => "Error updating institution";
    public string GetErrorMessage() => "An error occurred while updating the institution";
}