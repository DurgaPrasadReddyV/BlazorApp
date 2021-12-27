using BlazorApp.CommonInfrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.CommonInfrastructure.Persistence;
internal static class Startup
{
    internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        return services.AddDbContext<ApplicationDbContext>(
            m => m.UseSqlServer(config.GetConnectionString("DefaultConnection"), e => e.MigrationsAssembly("Migrators.MSSQL")));
    }
}