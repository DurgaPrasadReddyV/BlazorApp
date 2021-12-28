using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Exceptions;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.CommonInfrastructure.Identity;
using BlazorApp.CommonInfrastructure.Identity.Permissions;
using BlazorApp.CommonInfrastructure.Identity.Services;
using BlazorApp.CommonInfrastructure.Middleware;
using BlazorApp.CommonInfrastructure.Notifications;
using BlazorApp.Domain.Configurations;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddHttpApiInfrastructure(this IServiceCollection services, JwtSettings jwtSettings)
    {
        if (string.IsNullOrEmpty(jwtSettings.Key))
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        
        byte[] key = Encoding.ASCII.GetBytes(jwtSettings.Key);
        services.AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };
                bearer.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            throw new IdentityException("Authentication Failed.", statusCode: HttpStatusCode.Unauthorized);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = _ => throw new IdentityException("You are not authorized to access this resource.", statusCode: HttpStatusCode.Forbidden),
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/notifications"))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddSignalR();
        services.AddTransient<NotificationHub>();
        services.AddTransient<INotificationService,NotificationService>();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<CurrentUserMiddleware>();
        services.AddScoped<RequestLoggingMiddleware>();
        services.AddScoped<ResponseLoggingMiddleware>();

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllers().AddFluentValidation();
        
        return services;
    }

    public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app)
    {
        app.UseMiddleware<CurrentUserMiddleware>();

        return app;
    }
    
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ResponseLoggingMiddleware>();

        return app;
    }

    public static IEndpointRouteBuilder MapNotifications(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<NotificationHub>("/notifications", options =>
        {
            options.CloseOnAuthenticationExpiration = true;
        });
        return endpoints;
    }
}