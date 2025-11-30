using Microsoft.AspNetCore.Http;

namespace Common.Authorization;

public class UserHelper(IHttpContextAccessor httpContextAccessor)
{
    public string GetUserCompany()
    {
        var company = httpContextAccessor.HttpContext?.User?.FindFirst("company")?.Value ?? "";
        return company;
    }

    public string GetCompanyHeader()
    {
        var companyHeader = httpContextAccessor.HttpContext?.Request.Headers["company"].ToString();
        var company = !string.IsNullOrEmpty(companyHeader)
            ? companyHeader
            : throw new InvalidOperationException("Company header is missing.");
        return company;
    }
}