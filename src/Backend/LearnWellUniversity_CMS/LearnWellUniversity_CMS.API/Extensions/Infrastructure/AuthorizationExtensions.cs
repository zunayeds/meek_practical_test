using LearnWellUniversity_CMS.Shared.Constants;

namespace LearnWellUniversity_CMS.API.Extensions.Infrastructure;

public static class AuthorizationExtensions
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.StaffOnly, p => p.RequireRole(Roles.Staff));
            options.AddPolicy(Policies.StudentOnly, p => p.RequireRole(Roles.Student));
            options.AddPolicy(Policies.StuffOrStudent, p => p.RequireRole(Roles.Staff, Roles.Student));
        });

        return services;
    }
}