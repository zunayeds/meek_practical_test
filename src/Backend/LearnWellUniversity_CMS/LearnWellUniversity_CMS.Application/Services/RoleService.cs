using Microsoft.AspNetCore.Identity;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IRoleService
{
    Task CreateRoleAsync(string role, bool throwErrorIfExists = false);
}

public class RoleService(RoleManager<IdentityRole<Guid>> roleManager) : IRoleService
{
    public async Task CreateRoleAsync(string role, bool throwErrorIfExists = false)
    {
        if (await roleManager.RoleExistsAsync(role))
        {
            if (throwErrorIfExists) throw new Exception($"The role '{role}' already exists");
        }
        else
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}