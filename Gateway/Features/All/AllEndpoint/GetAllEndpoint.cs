using Microsoft.AspNetCore.Mvc;

namespace Gateway.Features.All.AllEndpoint;

internal static class AllInstitutionEndpoint
{
    public static async Task<IResult> GetAllAsync(
        [FromServices] ILogger<Program> logger,
        [FromServices] HttpClient httpClient,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        if (httpContextAccessor?.HttpContext == null)
        {
            return Results.BadRequest();
        }

        var authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        var institutionsTask = SendAndReadAsync<InstitutionsResponse>(httpClient, "https://localhost:20011/institutions", authHeader, logger, cancellationToken);
        var customersTask = SendAndReadAsync<CustomersResponse>(httpClient, "https://localhost:20011/customers", authHeader, logger, cancellationToken);
        var addressesTask = SendAndReadAsync<AddressesResponse>(httpClient, "https://localhost:20011/addresses", authHeader, logger, cancellationToken);
        var documentsTask = SendAndReadAsync<DocumentsResponse>(httpClient, "https://localhost:20011/documents", authHeader, logger, cancellationToken);

        await Task.WhenAll(institutionsTask, customersTask, addressesTask, documentsTask).ConfigureAwait(false);

        var institutionsData = await institutionsTask.ConfigureAwait(false);
        var customersData = await customersTask.ConfigureAwait(false);
        var addressesData = await addressesTask.ConfigureAwait(false);
        var documentsData = await documentsTask.ConfigureAwait(false);

        if (institutionsData == null || customersData == null || addressesData == null || documentsData == null)
        {
            logger.LogError("One or more downstream services failed to return successful responses.");
            return Results.BadRequest();
        }

        return Results.Ok(new
        {
            institutionsData,
            customersData,
            addressesData,
            documentsData
        });
    }

    private static async Task<T?> SendAndReadAsync<T>(HttpClient client, string url, string? authHeader, ILogger logger, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (!string.IsNullOrEmpty(authHeader))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "HTTP request to {Url} failed.", url);
            return default;
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Downstream service {Url} returned {Status} - {Reason}", url, response.StatusCode, response.ReasonPhrase);
            return default;
        }

        try
        {
            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize response from {Url}.", url);
            return default;
        }
    }
}