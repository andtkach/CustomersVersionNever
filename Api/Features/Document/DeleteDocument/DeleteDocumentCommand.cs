using Api.Abstractions;
using Common.Requests.Document;

namespace Api.Features.Document.DeleteDocument;

public sealed class DeleteDocumentCommand(Guid documentId) : IBaseCommand<DocumentDeletePayload>
{
    public string Action => "Delete";
    
    public Guid DocumentId { get; } = documentId;

    public DocumentDeletePayload Payload() => new()
    {
        Id = DocumentId
    };

    public string GetLogMessage() => "Error deleting document";
    public string GetErrorMessage() => "An error occurred while deleting the document";
}