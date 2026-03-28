using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;

namespace LearnWellUniversity_CMS.API.Extensions.Infrastructure
{
    public static class DataSeederExtensions
    {
        public static async Task SeedInitialDataAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                await DataSeeder.SeedAsync(userManager, roleManager);
            }
        }
    }
}
