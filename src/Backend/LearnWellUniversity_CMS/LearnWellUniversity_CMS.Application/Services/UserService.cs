using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Exceptions;
using LearnWellUniversity_CMS.Shared.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IUserService
{
    Task<(Guid Id, string Password)> CreateWithRoleAsync(ApplicationUser user, string role, string? password, bool throwErrorIfExists = false);
}

public class UserService(UserManager<ApplicationUser> userManager, ILogger<UserService> logger) : IUserService
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

            logger.LogInformation("Creating user '{FirstName} {LastName}'", user.FirstName, user.LastName);
            var createResult = await userManager.CreateAsync(user, password);
            logger.LogInformation("Created user '{FirstName} {LastName}'", user.FirstName, user.LastName);

            if (!createResult.Succeeded)
                throw new Exception(string.Join("; ", createResult.Errors.Select(e => e.Description)));

            logger.LogInformation("Adding '{role}' role to user '{FirstName} {LastName}'", role, user.FirstName, user.LastName);
            var addToRoleResult = await userManager.AddToRoleAsync(user, role);
            logger.LogInformation("Added '{role}' role to user '{FirstName} {LastName}'", role, user.FirstName, user.LastName);
            if (!addToRoleResult.Succeeded)
                throw new Exception(string.Join("; ", addToRoleResult.Errors.Select(e => e.Description)));
        }

        return (user.Id, password ?? string.Empty);
    }
}