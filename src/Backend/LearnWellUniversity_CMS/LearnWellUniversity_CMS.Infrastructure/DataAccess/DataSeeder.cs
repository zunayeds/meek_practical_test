using LearnWellUniversity_CMS.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace LearnWellUniversity_CMS.Infrastructure.DataAccess;

public class DataSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in new[] { "Staff", "Student" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        const string adminEmail = "admin@learnwell.edu";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var id = Guid.NewGuid();
            var admin = new ApplicationUser
            {
                Id = id,
                FirstName = "Super",
                LastName = "Admin",
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = id,
                IsActive = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Staff");
        }
    }
}
