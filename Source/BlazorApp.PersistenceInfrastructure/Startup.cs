using BlazorApp.Application.Common.Interfaces;
using BlazorApp.CommonInfrastructure.Persistence.Contexts;
using BlazorApp.Domain.Configurations;
using BlazorApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, ConnectionStrings connectionStrings)
    {
        services.AddTransient<IRepository, Repository>();

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionStrings.DefaultConnection!));
    
        return services;
    }
}