﻿using BlazorApp.Client.Authentication.Jwt;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace BlazorApp.Client.Authentication;

internal static class Startup
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        return services
            .AddScoped<AuthenticationStateProvider, JwtAuthenticationService>()
            .AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>())
            .AddScoped(sp => (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>())
            .AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
            .AddScoped<JwtAuthenticationHeaderHandler>();
    }

    public static IHttpClientBuilder AddAuthenticationHandler(this IHttpClientBuilder builder)
    {
        return builder.AddHttpMessageHandler<JwtAuthenticationHeaderHandler>();
    }
}