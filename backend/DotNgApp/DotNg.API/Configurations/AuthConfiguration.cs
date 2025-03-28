using DotNg.Application.Models.Auth;
using DotNg.Infrastructure.Authentication.Jwt.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DotNg.API.Configurations;

public static class AuthConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new();
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        var googleAuthSettings = configuration.GetSection("Authentication:Google").Get<GoogleAuthSettings>();
        var facebookAuthSettings = configuration.GetSection("Authentication:Facebook").Get<FacebookAuthSettings>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                }
            };
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/signin";
            options.AccessDeniedPath = "/access-denied";
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });

        if (googleAuthSettings is not null)
        {
            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = googleAuthSettings.ClientId;
                options.ClientSecret = googleAuthSettings.ClientSecret;
                options.CallbackPath = googleAuthSettings.CallbackPath;
            });
        }

        if (facebookAuthSettings is not null)
        {
            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = facebookAuthSettings.AppId;
                options.AppSecret = facebookAuthSettings.AppSecret;
                options.CallbackPath = facebookAuthSettings.CallbackPath;
            });
        }
    }
}