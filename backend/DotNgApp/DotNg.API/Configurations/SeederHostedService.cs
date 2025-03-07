using DotNg.Infrastructure.Authentication.Identity.Models;
using DotNg.Infrastructure.Data;
using DotNg.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNg.API.Configurations;

public class SeederHostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        await SeedCommonDataAsync(roleManager, userManager);
    }

    private static async Task SeedCommonDataAsync(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
    {
        await RoleSeeder.SeedRolesAsync(roleManager);
        await UserSeeder.SeedUsersAsync(userManager);
        await UserSeeder.SeedTestUsersAsync(userManager);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}