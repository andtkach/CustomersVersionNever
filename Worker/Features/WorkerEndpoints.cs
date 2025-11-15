using Worker.Features.Customer.GetCustomer;
using Worker.Features.customer.GetCustomers;
using Worker.Features.Institution.GetInstitution;
using Worker.Features.Institution.GetInstitutions;

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
}
