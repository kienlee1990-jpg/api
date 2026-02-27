using FastFoodAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFoodAPI.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedRolesAndAdminAsync(this IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = { "Admin", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = "admin@fastfood.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            var newAdmin = new ApplicationUser
            {
                Email = adminEmail,
                UserName = "admin",
                FullName = "System Admin",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(newAdmin, "Admin@123");
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}