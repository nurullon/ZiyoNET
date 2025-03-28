using DotNg.API.Infrastructure;
using DotNg.Application.Serialization;
using DotNg.Application.Services;
using DotNg.Application.Services.Auth;
using DotNg.Application.Services.Auth.Interfaces;
using DotNg.Application.Services.Interfaces;
using DotNg.Domain.Interfaces;
using DotNg.Infrastructure.Authentication.Identity;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Jwt;
using DotNg.Infrastructure.Security;

namespace DotNg.API.Configurations;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        // Exception Handling Services
        services.AddSingleton<ResponseSerializer>();

        // Add Exception Handler
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        //Seed Data
        services.AddHostedService<SeederHostedService>();

        //JWT
        services.AddScoped<IJwtService, JwtService>();

        //Password Hasher
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        //Identity
        services.AddScoped<IIdentityService, IdentityService>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        // Register Service
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IFacebookAuthService, FacebookAuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IExcelService, ExcelService>();
    }
}