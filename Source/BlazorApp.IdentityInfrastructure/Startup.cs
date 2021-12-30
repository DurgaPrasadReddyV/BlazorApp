using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.CommonInfrastructure.Identity.Services;
using BlazorApp.CommonInfrastructure.Mapping;
using BlazorApp.CommonInfrastructure.Persistence.Contexts;
using BlazorApp.Domain.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, ConnectionStrings connectionStrings)
    {
        MapsterSettings.Configure();

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IRoleClaimsService, RoleClaimsService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IUserService, UserService>();

        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionStrings.DefaultConnection!));
        
        services.AddIdentity<BlazorAppUser, BlazorAppRole>(options =>
        {
            options.Password.RequiredLength = 2;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<IdentityDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}