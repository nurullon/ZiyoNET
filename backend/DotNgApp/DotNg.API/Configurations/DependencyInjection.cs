using DotNg.API.Infrastructure;
using DotNg.Application.Services;
using DotNg.Application.Services.Auth;
using DotNg.Application.Services.Auth.Interfaces;
using DotNg.Domain.Interfaces;
using DotNg.Infrastructure.Authentication.Identity;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Models;
using DotNg.Infrastructure.Authentication.Jwt;
using DotNg.Infrastructure.Data;
using DotNg.Infrastructure.Security;
using DotNg.Infrastructure.Serialization;
using Microsoft.AspNetCore.Identity;

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
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IFacebookAuthService, FacebookAuthService>();
    }
}