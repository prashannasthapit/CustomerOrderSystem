using CustomerOrderSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CustomerOrderSystem.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        // 1. Seed Roles
        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>
                {
                    Name = role
                });
            }
        }

        // 2. Seed Admin User
        string adminEmail = "admin@test.com";
        string adminPassword = "Admin@123"; // change in production

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                throw new Exception("Failed to create admin user: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}