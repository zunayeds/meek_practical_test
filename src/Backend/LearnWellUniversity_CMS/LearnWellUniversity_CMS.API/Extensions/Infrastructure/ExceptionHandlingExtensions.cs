using LearnWellUniversity_CMS.API.Middlewares;

namespace LearnWellUniversity_CMS.API.Extensions.Infrastructure;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection ConfigureExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
