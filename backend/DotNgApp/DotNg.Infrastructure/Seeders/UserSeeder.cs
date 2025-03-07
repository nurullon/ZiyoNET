using DotNg.Infrastructure.Authentication.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace DotNg.Infrastructure.Seeders;

public class UserSeeder
{
    public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
    {
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser { 
                UserName = adminEmail, 
                Email = adminEmail, 
                Name = "Admin User",
                PasswordHash = HashPassword("password")
            };
            await userManager.CreateAsync(adminUser);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    public static async Task SeedTestUsersAsync(UserManager<AppUser> userManager)
    {
        var testUserEmail = "testuser@example.com";
        var testUser = await userManager.FindByEmailAsync(testUserEmail);
        if (testUser == null)
        {
            testUser = new AppUser 
            {
                UserName = testUserEmail, 
                Email = testUserEmail,
                Name = "Test User",
                PasswordHash = HashPassword("password")
            };
            await userManager.CreateAsync(testUser);
            await userManager.AddToRoleAsync(testUser, "User");
        }
    }

    private const int SaltSize = 16;
    private const int HashSize = 20;
    private const int Iterations = 10000;

    public static string HashPassword(string password)
    {
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations: Iterations, hashAlgorithm: HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(HashSize);

        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }
}