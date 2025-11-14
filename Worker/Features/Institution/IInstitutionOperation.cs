using Common.Model;
using Microsoft.Extensions.Caching.Hybrid;
using Worker.Data;

namespace Worker.Features.Institution;

public interface IInstitutionOperation
{
    Task ExecuteAsync(
        Intent intent,
        FrontendDataContext frontendDataContext,
        BackendDataContext backendDataContext,
        HybridCache hybridCache);
}