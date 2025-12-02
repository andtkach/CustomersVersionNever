using Common.Authorization;
using Storage.Features.Storage.DeleteFile;
using Storage.Features.Storage.GetFile;
using Storage.Features.Storage.UploadFile;

namespace Storage.Features;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapStorageEndpoints(this IEndpointRouteBuilder app)
    {
        
        app.MapPost("/storage/{fileId:guid}", UploadFileEndpoint.UploadFileAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite))
            .Accepts<IFormFile>("multipart/form-data")
            .DisableAntiforgery();
        
        app.MapDelete("/storage/{fileId:guid}", DeleteFileEndpoint.DeleteFileAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRemove));
        
        app.MapGet("/storage/{fileId:guid}", GetFileEndpoint.GetFileAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));
        
        return app;
    }
}
