using DotNg.Application.Models.Auth;
using DotNg.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace DotNg.Infrastructure.Authentication.Identity.Interfaces;

public interface IFacebookAuthService
{
    string GetFacebookLoginUrl(string? redirectUri);
    Task<Result<LoginResponse>> HandleFacebookLoginAsync(HttpContext httpContext);
}
