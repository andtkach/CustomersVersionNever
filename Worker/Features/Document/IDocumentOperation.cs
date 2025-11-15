using Common.Model;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data;

namespace Worker.Features.Document;

public interface IDocumentOperation
{
    Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache);
}