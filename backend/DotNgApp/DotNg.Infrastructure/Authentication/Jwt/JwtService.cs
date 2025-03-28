using DotNg.Infrastructure.Authentication.Identity.Models;
using DotNg.Infrastructure.Authentication.Jwt.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DotNg.Infrastructure.Authentication.Jwt;

public class JwtService(IOptions<JwtOptions> options, UserManager<AppUser> userManager) : IJwtService
{
    public string GenerateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
             new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.Name)
        };

        var userRoles = userManager.GetRolesAsync(user).Result;

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(10),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}