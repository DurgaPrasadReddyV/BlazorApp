using BlazorApp.Infrastructure.Caching;
using BlazorApp.Infrastructure.Common;
using BlazorApp.Infrastructure.Cors;
using BlazorApp.Infrastructure.FileStorage;
using BlazorApp.Infrastructure.Hangfire;
using BlazorApp.Infrastructure.Identity;
using BlazorApp.Infrastructure.Localization;
using BlazorApp.Infrastructure.Mailing;
using BlazorApp.Infrastructure.Mapping;
using BlazorApp.Infrastructure.Middleware;
using BlazorApp.Infrastructure.Notifications;
using BlazorApp.Infrastructure.Persistence;
using BlazorApp.Infrastructure.SecurityHeaders;
using BlazorApp.Infrastructure.Seeding;
using BlazorApp.Infrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        MapsterSettings.Configure();
        return services
            .AddApiVersioning()
            .AddCaching(config)
            .AddCorsPolicy(config)
            .AddCurrentUser()
            .AddExceptionMiddleware()
            .AddHangfire(config)
            .AddIdentity(config)
            .AddLocalization(config)
            .AddMailing(config)
            .AddNotifications(config)
            .AddPermissions()
            .AddRequestLogging(config)
            .AddRouting(options => options.LowercaseUrls = true)
            .AddSeeders()
            .AddServices()
            .AddSwaggerDocumentation(config)
            .AddDatabase(config);
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder appBuilder, IConfiguration config) =>
        appBuilder
            .UseLocalization(config)
            .UseStaticFiles()
            .UseSecurityHeaders(config)
            .UseFileStorage()
            .UseExceptionMiddleware()
            .UseLocalization(config)
            .UseRouting()
            .UseCorsPolicy()
            .UseAuthentication()
            .UseCurrentUser()
            .UseAuthorization()
            .UseRequestLogging(config)
            .UseHangfireDashboard(config)
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapNotifications();
            })
            .UseSwaggerDocumentation(config);

    private static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
}