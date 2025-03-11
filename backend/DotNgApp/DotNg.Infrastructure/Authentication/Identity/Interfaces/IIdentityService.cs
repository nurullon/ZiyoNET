using DotNg.Infrastructure.Authentication.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace DotNg.Infrastructure.Authentication.Identity.Interfaces;

public interface IIdentityService
{
    Task<AppUser?> FindByEmailAsync(string email);
    Task<IdentityResult> CreateUserAsync(AppUser user, string password);
    Task AddToRoleAsync(AppUser user, string role);
    Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user);
    Task AddLoginAsync(AppUser user, UserLoginInfo loginInfo);
    Task SignInAsync(AppUser user);
    Task<string?> GetAuthenticationTokenAsync(AppUser user, string tokenName);
    Task SetAuthenticationTokenAsync(AppUser user, string tokenName, string token);
    Task RemoveAuthenticationTokenAsync(AppUser user, string tokenName);
    IQueryable<AppUser> GetUsers();
}