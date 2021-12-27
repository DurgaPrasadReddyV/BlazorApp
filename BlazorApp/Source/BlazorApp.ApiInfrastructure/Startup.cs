using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Exceptions;
using BlazorApp.CommonInfrastructure.Identity;
using BlazorApp.CommonInfrastructure.Middleware;
using BlazorApp.CommonInfrastructure.Notifications;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddHttpApiInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddSignalR();
        services.AddTransient<NotificationHub>();
        services.AddTransient<INotificationService,NotificationService>();

        services.AddScoped<CurrentUserMiddleware>();
        services.AddScoped<RequestLoggingMiddleware>();
        services.AddScoped<ResponseLoggingMiddleware>();

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllers().AddFluentValidation();
        
        return services;
    }

    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
    {
        app.UseMiddleware<CurrentUserMiddleware>();

        return app;
    }
    
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ResponseLoggingMiddleware>();

        return app;
    }

    public static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<NotificationHub>("/notifications", options =>
        {
            options.CloseOnAuthenticationExpiration = true;
        });
        return endpoints;
    }
}