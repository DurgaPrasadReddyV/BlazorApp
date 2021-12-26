using BlazorApp.Application;
using BlazorApp.Host.Configurations;
using BlazorApp.Infrastructure;
using FluentValidation.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddConfigurations();
    builder.Host.UseSerilog((_, config) =>
    {
        config.WriteTo.Console()
        .ReadFrom.Configuration(builder.Configuration);
    });

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddControllersWithViews().AddFluentValidation();
    builder.Services.AddRazorPages();

    var app = builder.Build();

    app.UseBlazorFrameworkFiles();
    app.UseInfrastructure(builder.Configuration);
    app.MapFallbackToFile("index.html");

    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}
