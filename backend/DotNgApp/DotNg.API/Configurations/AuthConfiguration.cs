using DotNg.Application.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;

namespace DotNg.API.Configurations;

public static class AuthConfiguration
{
    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var googleAuthSettings = configuration.GetSection("Authentication:Google").Get<GoogleAuthSettings>();
        var facebookAuthSettings = configuration.GetSection("Authentication:Facebook").Get<FacebookAuthSettings>();

        if (googleAuthSettings is null || facebookAuthSettings is null)
            return;

        services.Configure<GoogleAuthSettings>(configuration.GetSection("Authentication:Google"));
        services.Configure<FacebookAuthSettings>(configuration.GetSection("Authentication:Facebook"));

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
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
        })
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = googleAuthSettings.ClientId;
            options.ClientSecret = googleAuthSettings.ClientSecret;
            options.CallbackPath = googleAuthSettings.CallbackPath;
        })
        .AddFacebook(options =>
        {
            options.AppId = facebookAuthSettings.AppId;
            options.AppSecret = facebookAuthSettings.AppSecret;
            options.CallbackPath = facebookAuthSettings.CallbackPath;
        });
    }
}