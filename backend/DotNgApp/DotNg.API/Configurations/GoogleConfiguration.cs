using DotNg.Application.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace DotNg.API.Configurations;

public static class GoogleConfiguration
{
    public static void ConfigureGoogle(this IServiceCollection services, IConfiguration configuration)
    {
        var googleAuthSettings = new GoogleAuthSettings();
        configuration.GetSection("GoogleAuth").Bind(googleAuthSettings);
        services.AddSingleton(googleAuthSettings);

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
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
        })
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = googleAuthSettings.ClientId;
            options.ClientSecret = googleAuthSettings.ClientSecret;
            options.CallbackPath = googleAuthSettings.CallbackPath;
            options.SaveTokens = true;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        });
    }
}