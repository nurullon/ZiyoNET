using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace DotNg.Infrastructure.Authentication.Identity;

public class IdentityService(UserManager<AppUser> userManager, 
    SignInManager<AppUser> signInManager) : IIdentityService
{
    public async Task<AppUser?> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityResult> CreateUserAsync(AppUser user, string password)
    {
        user.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user, password);
        return await userManager.CreateAsync(user);
    }

    public async Task AddToRoleAsync(AppUser user, string role)
    {
        await userManager.AddToRoleAsync(user, role);
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user)
    {
        return await userManager.GetLoginsAsync(user);
    }

    public async Task AddLoginAsync(AppUser user, UserLoginInfo loginInfo)
    {
        await userManager.AddLoginAsync(user, loginInfo);
    }

    public async Task SignInAsync(AppUser user)
    {
        await signInManager.SignInAsync(user, false);
    }

    public async Task<string?> GetAuthenticationTokenAsync(AppUser user, string tokenName)
    {
        return await userManager.GetAuthenticationTokenAsync(user, "JWT", tokenName);
    }

    public async Task SetAuthenticationTokenAsync(AppUser user, string tokenName, string token)
    {
        await userManager.SetAuthenticationTokenAsync(user, "JWT", tokenName, token);
    }

    public async Task RemoveAuthenticationTokenAsync(AppUser user, string tokenName)
    {
        await userManager.RemoveAuthenticationTokenAsync(user, "JWT", tokenName);
    }

    public IQueryable<AppUser> GetUsers()
    {
        return userManager.Users;
    }

    public async Task<string?> GetRoleAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }
}