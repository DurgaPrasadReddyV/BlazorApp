using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.FileStorage;
using BlazorApp.CommonInfrastructure.Common.Services;
using BlazorApp.CommonInfrastructure.FileStorage;
using BlazorApp.CommonInfrastructure.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IFileStorageService, LocalFileStorageService>();
        services.AddTransient<IEventService, EventService>();
        services.AddTransient<ISerializerService, NewtonSoftService>();
        return services;
    }

    public static IApplicationBuilder UseFileStorage(this IApplicationBuilder app) =>
    app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Files")),
        RequestPath = new PathString("/Files")
    });
}