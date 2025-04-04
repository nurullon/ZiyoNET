using Microsoft.AspNetCore.Identity;

namespace DotNg.Infrastructure.Authentication.Identity.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
}