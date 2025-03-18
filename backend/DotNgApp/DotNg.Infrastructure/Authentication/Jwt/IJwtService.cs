using DotNg.Infrastructure.Authentication.Identity.Models;

namespace DotNg.Infrastructure.Authentication.Jwt;

public interface IJwtService
{
    string GenerateToken(AppUser user);
    string GenerateRefreshToken();
}