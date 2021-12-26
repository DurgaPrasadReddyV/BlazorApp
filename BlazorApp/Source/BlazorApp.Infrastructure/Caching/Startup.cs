using BlazorApp.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Infrastructure.Caching;

internal static class Startup
{
    internal static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddTransient<ICacheService, LocalCacheService>();

        return services;
    }
}