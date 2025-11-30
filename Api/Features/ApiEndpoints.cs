using Api.Features.Customer.CreateCustomer;
using Api.Features.Customer.DeleteCustomer;
using Api.Features.Customer.GetCustomer;
using Api.Features.Customer.GetCustomers;
using Api.Features.Customer.PatchCustomer;
using Api.Features.Customer.UpdateCustomer;
using Api.Features.Institution.CreateInstitution;
using Api.Features.Institution.DeleteInstitution;
using Api.Features.Institution.GetInstitution;
using Api.Features.Institution.GetInstitutions;
using Api.Features.Institution.PatchInstitution;
using Api.Features.Institution.UpdateInstitution;
using Api.Features.Document.CreateDocument;
using Api.Features.Document.DeleteDocument;
using Api.Features.Document.GetDocument;
using Api.Features.Document.GetDocuments;
using Api.Features.Document.PatchDocument;
using Api.Features.Document.UpdateDocument;
using Api.Features.Address.CreateAddress;
using Api.Features.Address.DeleteAddress;
using Api.Features.Address.GetAddresses;
using Api.Features.Address.GetAddress;
using Api.Features.Address.PatchAddress;
using Api.Features.Address.UpdateAddress;
using Common.Authorization;

namespace Api.Features;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapInstitutionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/institutions", CreateInstitutionEndpoint.CreateInstitutionAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPut("/institutions/{institutionId:guid}", UpdateInstitutionEndpoint.UpdateInstitutionAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPatch("/institutions/{institutionId:guid}", PatchInstitutionEndpoint.PatchInstitutionAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapDelete("/institutions/{institutionId:guid}", DeleteInstitutionEndpoint.DeleteInstitutionAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRemove));
        app.MapGet("/institutions/{institutionId:guid}", GetInstitutionEndpoint.GetInstitutionAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));
        app.MapGet("/institutions", GetInstitutionsEndpoint.GetInstitutionsAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));

        app.MapPost("/customers", CreateCustomerEndpoint.CreateCustomerAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPut("/customers/{customerId:guid}", UpdateCustomerEndpoint.UpdateCustomerAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPatch("/customers/{customerId:guid}", PatchCustomerEndpoint.PatchCustomerAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapDelete("/customers/{customerId:guid}", DeleteCustomerEndpoint.DeleteCustomerAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRemove));
        app.MapGet("/customers/{customerId:guid}", GetCustomerEndpoint.GetCustomerAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));
        app.MapGet("/customers", GetCustomersEndpoint.GetCustomersAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));

        app.MapPost("/documents", CreateDocumentEndpoint.CreateDocumentAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPut("/documents/{documentId:guid}", UpdateDocumentEndpoint.UpdateDocumentAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPatch("/documents/{documentId:guid}", PatchDocumentEndpoint.PatchDocumentAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapDelete("/documents/{documentId:guid}", DeleteDocumentEndpoint.DeleteDocumentAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRemove));
        app.MapGet("/documents/{documentId:guid}", GetDocumentEndpoint.GetDocumentAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));
        app.MapGet("/documents", GetDocumentsEndpoint.GetDocumentsAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));

        app.MapPost("/addresses", CreateAddressEndpoint.CreateAddressAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPut("/addresses/{addressId:guid}", UpdateAddressEndpoint.UpdateAddressAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapPatch("/addresses/{addressId:guid}", PatchAddressEndpoint.PatchAddressAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataWrite));
        app.MapDelete("/addresses/{addressId:guid}", DeleteAddressEndpoint.DeleteAddressAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRemove));
        app.MapGet("/addresses/{addressId:guid}", GetAddressEndpoint.GetAddressAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));
        app.MapGet("/addresses", GetAddressesEndpoint.GetAddressesAsync)
            .RequireAuthorization(policy => policy.RequirePermission(Permissions.DataRead));

        return app;
    }
}
