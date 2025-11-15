using Api.Abstractions;
using Common.Requests.Institution;

namespace Api.Features.Institution.PatchInstitution;

public sealed class PatchInstitutionCommand(Guid institutionId, string? name, string? description)
    : IBaseCommand<InstitutionPatchPayload>
{
    public string Action => "Patch";
    
    public Guid InstitutionId { get; } = institutionId;
    public string? Name { get; } = name;
    public string? Description { get; } = description;

    public InstitutionPatchPayload Payload() => new()
    {
        Id = InstitutionId,
        Name = Name,
        Description = Description
    };

    public string GetLogMessage() => "Error patching institution";
    public string GetErrorMessage() => "An error occurred while patching the institution";
}