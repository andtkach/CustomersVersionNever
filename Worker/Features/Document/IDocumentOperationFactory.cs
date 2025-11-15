namespace Worker.Features.Document;

public interface IDocumentOperationFactory
{
    IDocumentOperation CreateOperation(string action);
}