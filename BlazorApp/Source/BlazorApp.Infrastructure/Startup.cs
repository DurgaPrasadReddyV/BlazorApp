using BlazorApp.Infrastructure.Caching;
using BlazorApp.Infrastructure.Common;
using BlazorApp.Infrastructure.FileStorage;
using BlazorApp.Infrastructure.Identity;
using BlazorApp.Infrastructure.Localization;
using BlazorApp.Infrastructure.Mapping;
using BlazorApp.Infrastructure.Middleware;
using BlazorApp.Infrastructure.Notifications;
using BlazorApp.Infrastructure.Persistence;
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
            .AddCaching()
            .AddCurrentUser()
            .AddExceptionMiddleware()
            .AddIdentity(config)
            .AddLocalization(config)
            .AddNotifications()
            .AddPermissions()
            .AddRequestLogging()
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
            .UseFileStorage()
            .UseExceptionMiddleware()
            .UseLocalization(config)
            .UseRouting()
            .UseAuthentication()
            .UseCurrentUser()
            .UseAuthorization()
            .UseRequestLogging()
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