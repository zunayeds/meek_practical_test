using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.DTOs.Requests;
using LearnWellUniversity_CMS.Application.DTOs.Responses;
using LearnWellUniversity_CMS.Domain.Models;
using LearnWellUniversity_CMS.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearnWellUniversity_CMS.Application.Services;

public interface IAuthService
{
    Task<AuthenticationResponse?> AuthenticateAsync(AuthenticationRequest request);
}

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<AuthenticationResponse?> AuthenticateAsync(AuthenticationRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return null;

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(AdditionalClaims.FirstName, user.FirstName),
            new(AdditionalClaims.LastName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(AdditionalClaims.Role, role));
        }

        if (roles.Contains(Roles.Student))
        {
            var studentId = await unitOfWork.Students.GetIdByUserIdAsync(user.Id);
            if (!studentId.Equals(Guid.Empty))
                claims.Add(new Claim(AdditionalClaims.StudentId, studentId.ToString()));
        }

        var token = BuildToken(claims, out var expiresAt);

        return new AuthenticationResponse
        {
            Email = user.Email!,
            Roles = roles,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    private string BuildToken(IEnumerable<Claim> claims, out DateTime expiresAt)
    {
        var key = config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
        var expiryMinutes = int.Parse(config["Jwt:ExpiryMinutes"] ?? "60");
        expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            config["Jwt:Issuer"], config["Jwt:Audience"],
            claims, expires: expiresAt, signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
