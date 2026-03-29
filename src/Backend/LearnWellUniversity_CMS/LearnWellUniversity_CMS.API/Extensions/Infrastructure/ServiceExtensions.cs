using LearnWellUniversity_CMS.Application.Abstractions;
using LearnWellUniversity_CMS.Application.Services;
using LearnWellUniversity_CMS.Infrastructure.Repositories;
using LearnWellUniversity_CMS.Infrastructure.Services;

namespace LearnWellUniversity_CMS.API.Extensions.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ICourseService, CourseService>();

        return services;
    }
}
