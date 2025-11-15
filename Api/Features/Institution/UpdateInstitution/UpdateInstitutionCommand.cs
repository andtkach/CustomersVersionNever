using Api.Features.Institution.Abstractions;
using Common.Requests;

namespace Api.Features.Institution.UpdateInstitution;

public sealed class UpdateInstitutionCommand(Guid institutionId, string name, string description)
    : IInstitutionCommand<InstitutionUpdatePayload>
{
    public string Action => "Update";
    
    public Guid InstitutionId { get; } = institutionId;
    public string Name { get; } = name;
    public string Description { get; } = description;

    public InstitutionUpdatePayload CreatePayload() => new()
    {
        Id = InstitutionId,
        Name = Name,
        Description = Description
    };

    public string GetLogMessage() => "Error updating institution";
    public string GetErrorMessage() => "An error occurred while updating the institution";
}