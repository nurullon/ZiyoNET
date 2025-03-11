using DotNg.Application.Models.Auth;
using DotNg.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace DotNg.Application.Services.Auth.Interfaces;

public interface IGoogleAuthService
{
    string GetGoogleLoginUrl(string? redirectUri);
    Task<Result<LoginResponse>> HandleGoogleLoginAsync(HttpContext httpContext);
}