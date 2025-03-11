using DotNg.Application.Models.Auth;
using DotNg.Application.Services.Auth.Interfaces;
using DotNg.Domain.Common;
using DotNg.Domain.Common.Errors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotNg.Application.Services.Auth;

public class GoogleAuthService(IAuthService authService) : IGoogleAuthService
{
    public string GetGoogleLoginUrl(string? redirectUri)
    {
        var properties = new AuthenticationProperties { RedirectUri = redirectUri };
        return $"/api/auth/google-login?redirectUri={redirectUri}";
    }

    public async Task<Result<LoginResponse>> HandleGoogleLoginAsync(HttpContext httpContext)
    {
        var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            return Result<LoginResponse>.Fail(new UnauthorizedError());
        }

        var user = authenticateResult.Principal;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return Result<LoginResponse>.Fail(new UnauthorizedError());


        var email = user.FindFirst(ClaimTypes.Email)?.Value;
        var name = user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

        if (string.IsNullOrEmpty(email))
            return Result<LoginResponse>.Fail(new UserError(ErrorCodes.NotFound, "Email is required for authentication."));

        var loginRequest = new ExternalLoginRequest { Email = email, Name = name };
        return await authService.ExternalLoginAsync(loginRequest, "Google");
    }
}