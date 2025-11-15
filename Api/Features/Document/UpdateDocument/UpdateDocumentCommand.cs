using Api.Abstractions;
using Common.Requests.Document;

namespace Api.Features.Document.UpdateDocument;

public sealed class UpdateDocumentCommand(Guid documentId, Guid customerId, Guid institutionId, string title, string content, bool active)
    : IBaseCommand<DocumentUpdatePayload>
{
    public string Action => "Update";
    
    public Guid DocumentId { get; } = documentId;
    public Guid CustomerId { get; } = customerId;
    public Guid InstitutionId { get; } = institutionId;
    public string Title { get; } = title;
    public string Content { get; } = content;
    public bool Active { get; } = active;

    public DocumentUpdatePayload Payload() => new()
    {
        Id = DocumentId,
        CustomerId = CustomerId,
        InstitutionId = InstitutionId,
        Title = Title,
        Content = Content,
        Active = Active
    };

    public string GetLogMessage() => "Error updating document";
    public string GetErrorMessage() => "An error occurred while updating the document";
}