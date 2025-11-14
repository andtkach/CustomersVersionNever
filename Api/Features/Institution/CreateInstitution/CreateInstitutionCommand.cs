using Api.Features.Institution.Abstractions;
using Common.Requests;

namespace Api.Features.Institution.CreateInstitution;

public sealed class CreateInstitutionCommand : IInstitutionCommand<InstitutionCreatePayload>
{
    public string Action => "Create";
    
    public Guid InstitutionId { get; }
    public string Name { get; }
    public string Description { get; }

    public CreateInstitutionCommand(Guid institutionId, string name, string description)
    {
        InstitutionId = institutionId;
        Name = name;
        Description = description;
    }

    public InstitutionCreatePayload CreatePayload() => new()
    {
        Id = InstitutionId,
        Name = Name,
        Description = Description
    };

    public string GetLogMessage() => "Error creating institution";
    public string GetErrorMessage() => "An error occurred while creating the institution";
}