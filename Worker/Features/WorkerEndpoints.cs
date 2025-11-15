using Worker.Features.Customer.GetCustomer;
using Worker.Features.Customer.GetCustomers;
using Worker.Features.Document.GetDocument;
using Worker.Features.Document.GetDocuments;
using Worker.Features.Institution.GetInstitution;
using Worker.Features.Institution.GetInstitutions;
using Worker.Features.Address.GetAddress;
using Worker.Features.Address.GetAddresses;

namespace Worker.Features;

public static class WorkerEndpoints
{
    public static IEndpointRouteBuilder MapInstitutionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("institutions/{institutionId:guid}", GetInstitutionEndpoint.GetInstitutionAsync);
        app.MapGet("institutions", GetInstitutionsEndpoint.GetInstitutionsAsync);

        return app;
    }

    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("customers/{customerId:guid}", GetCustomerEndpoint.GetCustomerAsync);
        app.MapGet("customers", GetCustomersEndpoint.GetCustomersAsync);

        return app;
    }

    public static IEndpointRouteBuilder MapDocumentsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("documents/{documentId:guid}", GetDocumentEndpoint.GetDocumentAsync);
        app.MapGet("documents", GetDocumentsEndpoint.GetDocumentsAsync);

        return app;
    }

    public static IEndpointRouteBuilder MapAddressesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("addresses/{addressId:guid}", GetAddressEndpoint.GetAddressAsync);
        app.MapGet("addresses", GetAddressesEndpoint.GetAddressesAsync);

        return app;
    }
}
