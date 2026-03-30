using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;

namespace LearnWellUniversity_CMS.Infrastructure.DataAccess;

public class DataSeeder(IUserService userService, IRoleService roleService) : IDataSeeder
{
    public async Task SeedAsync()
    {
        foreach (var role in new[] { Roles.Staff, Roles.Student })
        {
            await roleService.CreateRoleAsync(role);
        }

        const string adminEmail = "admin@learnwell.edu";
        const string adminPassword = "Admin@123";

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
            CreatedBy = id
        };

        await userService.CreateWithRoleAsync(admin, Roles.Staff, adminPassword);
    }
}
