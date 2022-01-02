using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace BlazorApp.CommonInfrastructure.Persistence.Contexts;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(args[0],
            npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(ApplicationDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
                npgsql.MigrationsHistoryTable("__Application_Migrations");
            });

        return new ApplicationDbContext(builder.Options);
    }
}