using DotNg.Infrastructure.Authentication.Jwt.Models;
using Microsoft.Extensions.Options;

namespace DotNg.API.Configurations.Jwt;

public class JwtOptionsSetup(IConfiguration configuration) : IConfigureOptions<JwtOptions>
{
    private const string SectionName = "Jwt";
    public void Configure(JwtOptions options)
    {
        configuration.GetSection(SectionName).Bind(options);
    }
}