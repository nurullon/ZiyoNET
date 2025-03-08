using DotNg.API.Infrastructure;
using DotNg.Domain.Interfaces;
using DotNg.Infrastructure.Authentication.Identity;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Jwt;
using DotNg.Infrastructure.Security;
using DotNg.Infrastructure.Serialization;

namespace DotNg.API.Configurations;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        // Exception Handling Services
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddSingleton<ResponseSerializer>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        // Add seeders
        services.AddHostedService<SeederHostedService>();

        // Register Service
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IFacebookAuthService, FacebookAuthService>();
    }
}