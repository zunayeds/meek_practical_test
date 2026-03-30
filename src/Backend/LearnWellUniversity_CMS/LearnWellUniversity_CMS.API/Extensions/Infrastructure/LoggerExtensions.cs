using Serilog;

namespace LearnWellUniversity_CMS.API.Extensions.Infrastructure;

public static class LoggerExtensions
{
    public static WebApplicationBuilder ConfigureStructuralLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .WriteTo.Seq(context.Configuration["Seq:Url"] ?? throw new InvalidOperationException("Seq:Url is not configured."));
        });

        return builder;
    }
}
