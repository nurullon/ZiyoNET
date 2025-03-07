using DotNg.Application.Models.Auth;
using DotNg.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace DotNg.Infrastructure.Authentication.Identity.Interfaces;

public interface IGoogleAuthService
{
    string GetGoogleLoginUrl(string? redirectUri);
    Task<Result<LoginResponse>> HandleGoogleLoginAsync(HttpContext httpContext);
}