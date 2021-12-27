using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.CommonInfrastructure.Middleware;

internal static class Startup
{
    internal static IServiceCollection AddRequestLogging(this IServiceCollection services)
    {
        services.AddSingleton<RequestLoggingMiddleware>();
        services.AddScoped<ResponseLoggingMiddleware>();

        return services;
    }

    internal static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ResponseLoggingMiddleware>();

        return app;
    }
}