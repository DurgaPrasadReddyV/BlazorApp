using System.Reflection;
using BlazorApp.Application.Dashboard;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Application;

public static class Startup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediatR(Assembly.GetExecutingAssembly());

        services.AddTransient<IStatsService, StatsService>();

        return services;
    }
}