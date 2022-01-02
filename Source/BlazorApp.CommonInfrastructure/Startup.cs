using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.FileStorage;
using BlazorApp.CommonInfrastructure.Common.Services;
using BlazorApp.CommonInfrastructure.FileStorage;
using BlazorApp.CommonInfrastructure.HangFire;
using BlazorApp.CommonInfrastructure.Mailing;
using BlazorApp.CommonInfrastructure.Serialization;
using BlazorApp.Domain.Configurations;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services,
        ConnectionStrings connectionStrings, HangfireSettings hangfireSettings)
    {
        services.AddTransient<IFileStorageService, LocalFileStorageService>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<ISerializerService, NewtonSoftService>();
        services.AddTransient<IJobService, HangfireService>();
        services.AddTransient<IMailService, SmtpMailService>();
        services.AddTransient<IEmailTemplateService, EmailTemplateService>();

        services.AddHangfireConsoleExtensions();
        services.AddHangfire((_, config) =>
        {
            config.UsePostgreSqlStorage(connectionStrings.DefaultConnection,
                new PostgreSqlStorageOptions()
                {
                    QueuePollInterval = hangfireSettings.QueuePollInterval,
                    InvisibilityTimeout = hangfireSettings.InvisibilityTimeout
                })
            .UseFilter(new LogJobFilter())
            .UseConsole();
        });

        services.AddHangfireServer(options =>
        {
            options.HeartbeatInterval = hangfireSettings.HeartbeatInterval;
            options.Queues = hangfireSettings.Queues?.ToArray();
            options.SchedulePollingInterval = hangfireSettings.SchedulePollingInterval;
            options.ServerCheckInterval = hangfireSettings.ServerCheckInterval;
            options.ServerName = hangfireSettings.ServerName;
            options.ServerTimeout = hangfireSettings.ServerTimeout;
            options.ShutdownTimeout = hangfireSettings.ShutdownTimeout;
            options.WorkerCount = hangfireSettings.WorkerCount;
        });

        return services;
    }

    public static IApplicationBuilder UseFileStorage(this IApplicationBuilder app)
    {
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Files")),
            RequestPath = new PathString("/Files")
        });

        return app;
    }
    

    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, HangfireSettings hangfireSettings)
    {
        app.UseHangfireDashboard(hangfireSettings.Route, new DashboardOptions
        {
            DashboardTitle = hangfireSettings.DashboardTitle,
            StatsPollingInterval = hangfireSettings.StatsPollingInterval,
            AppPath = hangfireSettings.AppPath
        });

        return app;
    }
}