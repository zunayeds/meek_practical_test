using Microsoft.OpenApi.Models;

namespace LearnWellUniversity_CMS.API.Extensions.Infrastructure
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection ConfigureSwaggerDoc(this IServiceCollection services)
        {
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Learn Well University - Course Management System", Version = "v1" });
            });

            return services;
        }

        public static WebApplication UseSwaggerDocWithUI(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            return app;
        }
    }
}
