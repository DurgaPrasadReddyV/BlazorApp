using System.Globalization;
using System.Reflection;
using BlazorApp.Client.ApiClient;
using BlazorApp.Client.Authentication;
using BlazorApp.Client.Notifications;
using BlazorApp.Client.Preferences;
using BlazorApp.Client.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

namespace BlazorApp.Client;

public static class Startup
{
    private const string ClientName = "FullStackHero.API";

    public static IServiceCollection AddClientServices(this IServiceCollection services, WebAssemblyHostBuilder builder) =>
        services
            .AddLocalization(options => options.ResourcesPath = "Resources")
            .AddBlazoredLocalStorage()
            .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = false;
                })
            .AddScoped<IClientPreferenceManager, ClientPreferenceManager>()
            .AutoRegisterInterfaces<IAppService>()
            .AutoRegisterInterfaces<IApiService>()
            .AddAuthentication()
            .AddAuthorizationCore(RegisterPermissionClaims)
            .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
                .AddAuthenticationHandler()
                .Services
            .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName))
            .AddScoped(sp => new NotificationClient(sp.GetRequiredService<IAccessTokenProvider>(), sp.GetRequiredService<IAuthenticationService>(), builder))
            .AddScoped(sp => sp.GetRequiredService<NotificationClient>().HubConnection);

    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var prop in typeof(FSHPermissions)
            .GetNestedTypes()
            .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            if (prop.GetValue(null)?.ToString() is string permission)
            {
                options.AddPolicy(permission, policy => policy.RequireClaim(FSHClaims.Permission, permission));
            }
        }
    }

    public static IServiceCollection AutoRegisterInterfaces<T>(this IServiceCollection services)
    {
        var @interface = typeof(T);

        var types = @interface
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterface($"I{t.Name}"),
                Implementation = t
            })
            .Where(t => t.Service != null);

        foreach (var type in types)
        {
            if (@interface.IsAssignableFrom(type.Service))
            {
                services.AddTransient(type.Service, type.Implementation);
            }
        }

        return services;
    }
}