using BlazorApp.Domain.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace BlazorApp.CommonInfrastructure.Persistence.Contexts;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(args[0],
            npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(IdentityDbContextFactory).GetTypeInfo().Assembly.GetName().Name);
                npgsql.MigrationsHistoryTable("__Identity_Migrations");
            });

        return new IdentityDbContext(builder.Options);
    }
}