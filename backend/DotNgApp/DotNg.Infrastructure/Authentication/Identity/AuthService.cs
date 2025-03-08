using DotNg.Application.Models.Auth;
using DotNg.Domain.Common;
using DotNg.Domain.Common.Errors;
using DotNg.Domain.Common.Messages;
using DotNg.Domain.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Models;
using DotNg.Infrastructure.Authentication.Jwt;
using Microsoft.AspNetCore.Identity;

namespace DotNg.Infrastructure.Authentication.Identity;

public class AuthService(UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) : IAuthService
{
    public async Task<Result<LoginResponse>> RegisterAsync(RegisterRequest model)
    {
        var userExists = await userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.AlreadyExists, "User already exists"));

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            Name = model.Name,
            PasswordHash = passwordHasher.HashPassword(model.Password),
        };
        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded)
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.ValidationError, result.Errors.First().Description));

        await userManager.AddToRoleAsync(user, "User");

        var token = jwtService.GenerateToken(user);

        await SaveUserToken(user, token, "JWT");

        var response = new LoginResponse
        {
            Token = token,
            Name = user.Name,
            Email = user.Email
        };

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.NotFound, UserErrorMessages.UserNotFound));

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result.Fail<LoginResponse>(new UserError(ErrorCodes.PasswordMismatch, UserErrorMessages.InvalidCredentials));

        var token = jwtService.GenerateToken(user);

        await SaveUserToken(user, token, "JWT");

        var response = new LoginResponse
        {
            Token = token,
            Name = user.Name,
            Email = user.Email
        };

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<LoginResponse>> ExternalLoginAsync(ExternalLoginRequest request, string provider)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Name,
            };
            await userManager.CreateAsync(user);
        }

        var userLoginInfo = await userManager.GetLoginsAsync(user);
        if (!userLoginInfo.Any(l => l.LoginProvider == provider))
        {
            var loginInfo = new UserLoginInfo(provider, request.Email, provider);
            await userManager.AddLoginAsync(user, loginInfo);
        }

        await signInManager.SignInAsync(user, false);
        var token = jwtService.GenerateToken(user);

        await SaveUserToken(user, token, "JWT");

        var response = new LoginResponse
        {
            Token = token,
            Name = user.Name,
            Email = user.Email
        };

        return Result<LoginResponse>.Success(response);
    }

    private async Task SaveUserToken(AppUser user, string token, string tokenName)
    {
        var existingToken = await userManager.GetAuthenticationTokenAsync(user, "JWT", tokenName);
        if (!string.IsNullOrEmpty(existingToken))
        {
            await userManager.RemoveAuthenticationTokenAsync(user, "JWT", tokenName);
        }

        await userManager.SetAuthenticationTokenAsync(user, "JWT", tokenName, token);
    }
}