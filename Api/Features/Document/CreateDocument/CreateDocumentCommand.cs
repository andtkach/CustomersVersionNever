using Api.Abstractions;
using Common.Requests.Document;

namespace Api.Features.Document.CreateDocument;

public sealed class CreateDocumentCommand(Guid documentId, Guid customerId, Guid institutionId, string title, string content, bool active)
    : IBaseCommand<DocumentCreatePayload>
{
    public string Action => "Create";
    
    public Guid DocumentId { get; } = documentId;
    public Guid CustomerId { get; } = customerId;
    public string Title { get; } = title;
    public string Content { get; } = content;
    public bool Active { get; } = active;

    public DocumentCreatePayload Payload() => new()
    {
        Id = DocumentId,
        CustomerId = CustomerId,
        Title = Title,
        Content = Content,
        Active = Active
    };

    public string GetLogMessage() => "Error creating document";
    public string GetErrorMessage() => "An error occurred while creating the document";
}