using Api.Features.Institution.Abstractions;
using Common.Requests;

namespace Api.Features.Institution.PatchInstitution;

public sealed class PatchInstitutionCommand : IInstitutionCommand<InstitutionPatchPayload>
{
    public string Action => "Patch";
    
    public Guid InstitutionId { get; }
    public string? Name { get; }
    public string? Description { get; }

    public PatchInstitutionCommand(Guid institutionId, string? name, string? description)
    {
        InstitutionId = institutionId;
        Name = name;
        Description = description;
    }

    public InstitutionPatchPayload CreatePayload() => new()
    {
        Id = InstitutionId,
        Name = Name,
        Description = Description
    };

    public string GetLogMessage() => "Error patching institution";
    public string GetErrorMessage() => "An error occurred while patching the institution";
}