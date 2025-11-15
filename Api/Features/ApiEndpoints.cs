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

namespace Api.Features;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapInstitutionsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/institutions", CreateInstitutionEndpoint.CreateInstitutionAsync);
        app.MapPut("/institutions/{institutionId:guid}", UpdateInstitutionEndpoint.UpdateInstitutionAsync);
        app.MapPatch("/institutions/{institutionId:guid}", PatchInstitutionEndpoint.PatchInstitutionAsync);
        app.MapDelete("/institutions/{institutionId:guid}", DeleteInstitutionEndpoint.DeleteInstitutionAsync);
        app.MapGet("/institutions/{institutionId:guid}", GetInstitutionEndpoint.GetInstitutionAsync);
        app.MapGet("/institutions", GetInstitutionsEndpoint.GetInstitutionsAsync);

        app.MapPost("/customers", CreateCustomerEndpoint.CreateCustomerAsync);
        app.MapPut("/customers/{customerId:guid}", UpdateCustomerEndpoint.UpdateCustomerAsync);
        app.MapPatch("/customers/{customerId:guid}", PatchCustomerEndpoint.PatchCustomerAsync);
        app.MapDelete("/customers/{customerId:guid}", DeleteCustomerEndpoint.DeleteCustomerAsync);
        app.MapGet("/customers/{customerId:guid}", GetCustomerEndpoint.GetCustomerAsync);
        app.MapGet("/customers", GetCustomersEndpoint.GetCustomersAsync);
        
        return app;
    }
}
