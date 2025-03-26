using AutoMapper;
using DotNg.Application.Models.Auth;
using DotNg.Application.Services.Auth.Interfaces;
using DotNg.Domain.Common;
using DotNg.Domain.Common.Errors;
using DotNg.Domain.Common.Messages;
using DotNg.Domain.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Models;
using DotNg.Infrastructure.Authentication.Jwt;
using Microsoft.AspNetCore.Identity;

namespace DotNg.Application.Services.Auth;

public class AuthService(IIdentityService identityService, 
    IPasswordHasher passwordHasher, 
    IMapper mapper,
    IJwtService jwtService) : IAuthService
{
    public async Task<Result<LoginResponse>> RegisterAsync(RegisterRequest model)
    {
        var userExists = await identityService.FindByEmailAsync(model.UserName);
        if (userExists != null)
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.AlreadyExists, UserErrorMessages.UserAlreadyExists));

        var user = new AppUser
        {
            UserName = model.UserName,
            Email = model.UserName,
            Name = model.Name ?? string.Empty
        };

        var result = await identityService.CreateUserAsync(user, model.Password);
        if (!result.Succeeded)
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.ValidationError, result.Errors.First().Description));

        await identityService.AddToRoleAsync(user, "User");

        var token = jwtService.GenerateToken(user);
        await SaveUserToken(user, token);

        var response = new LoginResponse
        {
            Token = token,
            User = mapper.Map<CustomUserResponse>(user)
        };

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await identityService.FindByEmailAsync(request.UserName);
        if (user == null)
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.NotFound, UserErrorMessages.UserNotFound));

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.PasswordMismatch, UserErrorMessages.InvalidCredentials));

        var token = jwtService.GenerateToken(user);
        await SaveUserToken(user, token);

        var response = new LoginResponse
        {
            Token = token,
            User = mapper.Map<CustomUserResponse>(user)
        };

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<LoginResponse>> ExternalLoginAsync(ExternalLoginRequest request, string provider)
    {
        var user = await identityService.FindByEmailAsync(request.Email);
        if (user == null)
        {
            user = new AppUser { UserName = request.Email, Email = request.Email, Name = request.Name };
            await identityService.CreateUserAsync(user, string.Empty);
        }

        var logins = await identityService.GetLoginsAsync(user);
        if (!logins.Any(l => l.LoginProvider == provider))
        {
            var loginInfo = new UserLoginInfo(provider, request.Email, provider);
            await identityService.AddLoginAsync(user, loginInfo);
        }

        await identityService.SignInAsync(user);
        var token = jwtService.GenerateToken(user);
        await SaveUserToken(user, token);

        var response = new LoginResponse
        {
            Token = token,
            User = mapper.Map<CustomUserResponse>(user)
        };

        return Result<LoginResponse>.Success(response);
    }

    private async Task SaveUserToken(AppUser user, string token)
    {
        var existingToken = await identityService.GetAuthenticationTokenAsync(user, "JWT");
        if (!string.IsNullOrEmpty(existingToken))
        {
            await identityService.RemoveAuthenticationTokenAsync(user, "JWT");
        }

        await identityService.SetAuthenticationTokenAsync(user, "JWT", token);
    }
}

