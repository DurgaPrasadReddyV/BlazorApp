using BlazorApp.Application;
using BlazorApp.CommonInfrastructure;
using BlazorApp.Host.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    });

    builder.Host.UseSerilog((_, config) =>
    {
        config.ReadFrom.Configuration(builder.Configuration);
    });

    var jwtSettings = builder.Services.AddJwtSettings(builder.Configuration);
    var connectionStrings = builder.Services.AddConnectionStrings(builder.Configuration);

    builder.Services.AddApplication();
    builder.Services.AddCommonInfrastructure();
    builder.Services.AddIdentityInfrastructure(connectionStrings);
    builder.Services.AddHttpApiInfrastructure(jwtSettings);
    
    builder.Services.AddRazorPages();

    var app = builder.Build();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();
    app.UseFileStorage();
    app.UseRouting();
    app.UseAuthentication();
    app.UseCurrentUser();
    app.UseAuthorization();
    app.UseRequestLogging();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers().RequireAuthorization();
        endpoints.MapNotifications();
    });

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
