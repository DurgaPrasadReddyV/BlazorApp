using BlazorApp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Infrastructure.Caching;

internal static class Startup
{
    internal static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        services.AddMemoryCache();
        services.AddTransient<ICacheService, LocalCacheService>();

        return services;
    }
}