using LearnWellUniversity_CMS.Application.Services;

namespace LearnWellUniversity_CMS.API.Extensions.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
