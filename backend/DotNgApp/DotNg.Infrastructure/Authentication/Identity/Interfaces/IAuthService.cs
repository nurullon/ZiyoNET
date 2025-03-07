using DotNg.Application.Models.Auth;
using DotNg.Domain.Common;

namespace DotNg.Infrastructure.Authentication.Identity.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponse>> RegisterAsync(RegisterRequest model);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest model);
    Task<Result<LoginResponse>> ExternalLoginAsync(ExternalLoginRequest model);
}