using BlazorApp.Application;
using BlazorApp.CommonInfrastructure;
using BlazorApp.Domain.Identity;
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

    var jwtSettings = builder.Services.LoadJwtSettings(builder.Configuration);
    var connectionStrings = builder.Services.LoadConnectionStrings(builder.Configuration);
    var swaggerSettings = builder.Services.LoadSwaggerSettings(builder.Configuration);
    var corsSettings = builder.Services.LoadCorsSettings(builder.Configuration);

    builder.Services.AddApplication();
    builder.Services.AddCommonInfrastructure();
    builder.Services.AddIdentityInfrastructure(connectionStrings);
    builder.Services.AddHttpApiInfrastructure(jwtSettings, swaggerSettings, corsSettings);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();
    app.UseFileStorage();
    app.UseRouting();
    app.UseCors(ApiConstants.CorsPolicy);
    app.UseAuthentication();
    app.UseCurrentUser();
    app.UseAuthorization();
    app.UseRequestLogging();
    app.UseEndpoints(endpoints =>
    { 
        endpoints.MapControllers().RequireAuthorization();
        endpoints.MapNotifications();
        endpoints.MapFallbackToFile("index.html");
    });

    app.UseOpenApi();
    app.UseSwaggerUi3(options =>
    {
        options.DefaultModelsExpandDepth = -1;
        options.DocExpansion = "none";
        options.TagsSorter = "alpha";
    });

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
