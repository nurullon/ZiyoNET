using DotNg.Infrastructure.Authentication.Identity.Interfaces;
using DotNg.Infrastructure.Authentication.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DotNg.Infrastructure.Authentication.Identity;

public class IdentityService(UserManager<AppUser> userManager, 
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager) : IIdentityService
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

    public async Task<bool> UpdateUserRoleAsync(AppUser user, string newRole)
    {
        var currentRoles = await userManager.GetRolesAsync(user);
        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) return false;

        var addResult = await userManager.AddToRoleAsync(user, newRole);
        return addResult.Succeeded;
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

    public async Task<Role?> GetRoleAsync(AppUser user)
    {
        var roleNames = await userManager.GetRolesAsync(user);

        var roles = roleManager.Roles
            .Where(role => roleNames.Contains(role.Name ?? string.Empty))
            .Select(r => new Role
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            });

        return roles.FirstOrDefault();
    }

    public IQueryable<Role> GetRoles()
    {
        var rolesWithId = roleManager.Roles.Select(role => new Role { Id = role.Id, Name = role.Name ?? string.Empty });
        return rolesWithId;
    }

    public async Task<bool> DeleteUserAsync(AppUser user)
    {
        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
            return true;
        
        return false;
    }

    public async Task UpdateUserAsync(AppUser user)
    {
        await userManager.UpdateAsync(user);
    }
}