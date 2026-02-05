using AvansMeals.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace AvansMeals.Web.Seed;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Student", "CanteenEmployee" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    public static async Task SeedCanteenEmployeeAsync(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    string email,
    string password,
    int canteenId)
    {
        const string roleName = "CanteenEmployee";

        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new IdentityRole(roleName));

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                CanteenId = canteenId
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new Exception("CanteenEmployee user aanmaken faalde: " +
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            if (user.CanteenId != canteenId)
            {
                user.CanteenId = canteenId;
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new Exception("CanteenEmployee user updaten faalde: " +
                        string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                }
            }
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
            await userManager.AddToRoleAsync(user, roleName);
    }

}
