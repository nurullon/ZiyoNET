using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace DotNg.API.Configurations.Jwt;

public static class JwtConfiguration
{
    public static void ConfigureJWTService(this IServiceCollection services)
    {
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
    }
}