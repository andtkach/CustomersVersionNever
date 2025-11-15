using Api.Abstractions;
using Common.Requests.Document;

namespace Api.Features.Document.PatchDocument;

public sealed class PatchDocumentCommand(Guid documentId, Guid? customerId, Guid? institutionId, string? title, string? content, bool? active)
    : IBaseCommand<DocumentPatchPayload>
{
    public string Action => "Patch";
    
    public Guid DocumentId { get; } = documentId;
    public Guid? CustomerId { get; } = customerId;
    public Guid? InstitutionId { get; } = institutionId;
    public string? Title { get; } = title;
    public string? Content { get; } = content;
    public bool? Active { get; } = active;

    public DocumentPatchPayload Payload() => new()
    {
        Id = DocumentId,
        CustomerId = CustomerId,
        InstitutionId = InstitutionId,
        Title = Title,
        Content = Content,
        Active = Active
    };

    public string GetLogMessage() => "Error patching document";
    public string GetErrorMessage() => "An error occurred while patching the document";
}