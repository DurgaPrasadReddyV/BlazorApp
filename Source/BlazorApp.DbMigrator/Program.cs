using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.CommonInfrastructure.Persistence.Contexts;
using BlazorApp.DbMigrator.Identity.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

var services = new ServiceCollection();
services.AddLogging();
services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(args[0],
        npgsql =>
        {
            npgsql.MigrationsAssembly(typeof(IdentityDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
            npgsql.MigrationsHistoryTable("__Identity_Migrations");
        });
});

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(args[0],
        npgsql =>
        {
            npgsql.MigrationsAssembly(typeof(ApplicationDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
            npgsql.MigrationsHistoryTable("__Application_Migrations");
        });
});

services.AddIdentity<BlazorAppIdentityUser, BlazorAppIdentityRole>(options =>
{
    options.Password.RequiredLength = 2;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();

var serviceProvider = services.BuildServiceProvider();

using (var migrationScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using (var context = migrationScope.ServiceProvider.GetRequiredService<IdentityDbContext>())
    {
        await context.Database.MigrateAsync();
    }

    using (var context = migrationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
    {
        await context.Database.MigrateAsync();
    }
}

using (var seedScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = seedScope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var userManager = seedScope.ServiceProvider.GetRequiredService<UserManager<BlazorAppIdentityUser>>();
    var roleManager = seedScope.ServiceProvider.GetRequiredService<RoleManager<BlazorAppIdentityRole>>();

    foreach (var role in BlazorAppRoles.Get())
    {
        if (!await roleManager.RoleExistsAsync(role.Name))
        {
            var result = await roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                foreach (var claim in BlazorAppRoleClaims.Get())
                {
                    await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.ClaimType, claim.ClaimValue));
                }
            }
        }
    }

    foreach (var user in BlazorAppUsers.Get())
    {
        var userByEmail = await userManager.FindByEmailAsync(user.Email);

        if (userByEmail != default)
        {
            continue;
        }

        var result = await userManager.CreateAsync(user, "admin");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "admin");
        }
    }
}


