using Microsoft.AspNetCore.Identity;

namespace Auth.Data;

public sealed class ApplicationUser : IdentityUser
{
    public bool EnableNotifications { get; set; }
    public string Company { get; set; } = string.Empty;
}