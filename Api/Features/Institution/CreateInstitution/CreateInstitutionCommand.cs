using Api.Abstractions;
using Common.Requests.Institution;

namespace Api.Features.Institution.CreateInstitution;

public sealed class CreateInstitutionCommand(Guid institutionId, string name, string description)
    : IBaseCommand<InstitutionCreatePayload>
{
    public string Action => "Create";
    
    public Guid InstitutionId { get; } = institutionId;
    public string Name { get; } = name;
    public string Description { get; } = description;

    public InstitutionCreatePayload Payload() => new()
    {
        Id = InstitutionId,
        Name = Name,
        Description = Description
    };

    public string GetLogMessage() => "Error creating institution";
    public string GetErrorMessage() => "An error occurred while creating the institution";
}