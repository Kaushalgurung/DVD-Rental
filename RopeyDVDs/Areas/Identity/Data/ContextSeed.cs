using Microsoft.AspNetCore.Identity;
using RopeyDVDs.Enums;

namespace RopeyDVDs.Areas.Identity.Data;

public static class ContextSeed
{
    public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Seed Roles
        await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Assistant.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
    }

    public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Seed Default User
        var defaultUser = new ApplicationUser
        {
            UserName = "Santosh",
            FirstName = "Santosh",
            LastName = "Dangal",
            Email = "santoshdangal79@gmail.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            LockoutEnabled = false
        };
        if (userManager.Users.All(u => u.Id != defaultUser.Id))
        {
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Santosh!1");
                await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
                await userManager.AddToRoleAsync(defaultUser, Roles.Manager.ToString());
                await userManager.AddToRoleAsync(defaultUser, Roles.Assistant.ToString());
            }
        }
    }
}
