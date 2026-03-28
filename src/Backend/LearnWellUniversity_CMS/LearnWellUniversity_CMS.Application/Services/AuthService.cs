using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IAuthService
{
    Task<AuthenticationResponse?> AuthenticateAsync(AuthenticationRequest request);
}

public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
{
    public async Task<AuthenticationResponse?> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user is null || !user.IsActive || !await userManager.CheckPasswordAsync(user, request.Password))
            return null;

        var roles = await userManager.GetRolesAsync(user);

        return new AuthenticationResponse
        {
            Email = user.Email!,
            Roles = roles
        };
    }
}
