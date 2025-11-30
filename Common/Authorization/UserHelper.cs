using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

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

    public string GetCompanyForCache()
    {
        var company = this.GetUserCompany();
        return "-" + this.NormalizeString(company);
    }

    public string NormaliseCompanyForCache(string company)
    {
        return "-" + this.NormalizeString(company);
    }

    private string NormalizeString(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentNullException(nameof(input));
        }

        var trimmedInput = input.Trim();
        var lowerCaseInput = trimmedInput.ToLower();
        var cleanedInput = Regex.Replace(lowerCaseInput, "[^a-z]", "");
        return cleanedInput;
    }
}