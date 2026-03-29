using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Infrastructure.DataAccess;

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

                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
                await DataSeeder.SeedAsync(userService, roleService);
            }
        }
    }
}
