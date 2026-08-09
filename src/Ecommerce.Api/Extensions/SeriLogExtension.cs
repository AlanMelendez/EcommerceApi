using Serilog;
namespace Ecommerce.Api.Extensions
{

    //Use IServiceCollection extensions for services.
    //Use WebApplicationBuilder extensions when you need builder.Host.
    //Use WebApplication extensions for middleware.
    public static class SeriLogExtension
    {
        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            // Implementation for adding API rate limiting
            // This replaces the default ASP.NET Core logging provider with Serilog.
            // It reads the Serilog configuration from appsettings.Development.json or appsettings.json.

            builder.Host.UseSerilog((context, configuration) =>
            
              configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
            );

            return builder;
        }


        public static WebApplication UseApiSerilogRequestLogging(
        this WebApplication app)
        {
            // This logs one clean event for each HTTP request.
            // Example:
            // HTTP POST /api/auth/login responded 401 in 45 ms

            app.UseSerilogRequestLogging();

            return app;
            // Returning app allows method chaining.
        }
    }
}
