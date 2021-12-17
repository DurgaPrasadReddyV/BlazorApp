using BlazorApp.WebUI.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class BlazorAppDbContext : IdentityDbContext<BlazorAppUser, IdentityRole<Guid>, Guid>
{
    public BlazorAppDbContext(DbContextOptions<BlazorAppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}

public class BlazorAppDbContextFactory : IDesignTimeDbContextFactory<BlazorAppDbContext>
{
    public BlazorAppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BlazorAppDbContext>();
        optionsBuilder.UseNpgsql(@"Server=localhost;Port=5432;Database=blazorapp;User Id=postgres;Password=admin");

        return new BlazorAppDbContext(optionsBuilder.Options);
    }
}