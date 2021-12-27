using BlazorApp.CommonInfrastructure.Common;
using BlazorApp.CommonInfrastructure.FileStorage;
using BlazorApp.CommonInfrastructure.Identity;
using BlazorApp.CommonInfrastructure.Mapping;
using BlazorApp.CommonInfrastructure.Middleware;
using BlazorApp.CommonInfrastructure.Notifications;
using BlazorApp.CommonInfrastructure.Persistence;
using BlazorApp.CommonInfrastructure.Seeding;
using BlazorApp.CommonInfrastructure.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        MapsterSettings.Configure();
        return services
            .AddApiVersioning()
            .AddCurrentUser()
            .AddIdentity(config)
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
            .UseStaticFiles()
            .UseFileStorage()
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