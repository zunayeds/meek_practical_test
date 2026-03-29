using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using Microsoft.AspNetCore.Identity;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IUserService
{
    Task<(Guid Id, string Password)> CreateWithRoleAsync(ApplicationUser user, string role, string? password, bool throwErrorIfExists = false);
}

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<(Guid Id, string Password)> CreateWithRoleAsync(ApplicationUser user, string role, string? password, bool throwErrorIfExists = false)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email ?? string.Empty);

        if (existingUser is not null)
        {
            if (throwErrorIfExists) throw new AlreadyExistException($"The email '{user.Email}' is already been used for an user");
            return (existingUser.Id, password ?? string.Empty);
        }
        else
        {
            if (password is null)
            {
                password = PasswordGenerator.Generate();
            }
            var createResult = await userManager.CreateAsync(user, password);

            if(!createResult.Succeeded)
                throw new Exception(string.Join("; ", createResult.Errors.Select(e => e.Description)));

            var addToRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
                throw new Exception(string.Join("; ", addToRoleResult.Errors.Select(e => e.Description)));
        }

        return (user.Id, password ?? string.Empty);
    }
}