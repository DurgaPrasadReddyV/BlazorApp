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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema.Generation.TypeMappers;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Net;
using System.Security.Claims;
using System.Text;
using ZymLabs.NSwag.FluentValidation;
using BlazorApp.Domain.Identity;
using BlazorApp.Host.Controllers;

[assembly: ApiConventionType(typeof(FSHApiConventions))]

namespace BlazorApp.CommonInfrastructure;

public static class Startup
{
    public static IServiceCollection AddHttpApiInfrastructure(this IServiceCollection services,
        JwtSettings jwtSettings, SwaggerSettings swaggerSettings, CorsSettings corsSettings)
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
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };
                bearer.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

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
        services.AddTransient<INotificationService, NotificationService>();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<CurrentUserMiddleware>();
        services.AddScoped<RequestLoggingMiddleware>();
        services.AddScoped<ResponseLoggingMiddleware>();
        services.AddScoped<ExceptionMiddleware>();

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllers().AddFluentValidation();

        services.AddVersionedApiExplorer(o => o.SubstituteApiVersionInUrl = true);
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument((document, serviceProvider) =>
        {
            document.PostProcess = doc =>
            {
                doc.Info.Title = swaggerSettings.Title;
                doc.Info.Version = swaggerSettings.Version;
                doc.Info.Description = swaggerSettings.Description;
            };

            document.AddSecurity("bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Input your Bearer token to access this API",
                In = OpenApiSecurityApiKeyLocation.Header,
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
            });

            document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
            document.TypeMappers.Add(new PrimitiveTypeMapper(typeof(TimeSpan), schema =>
            {
                schema.Type = NJsonSchema.JsonObjectType.String;
                schema.IsNullableRaw = true;
                schema.Pattern = @"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$";
                schema.Example = "02:00:00";
            }));

            var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetService<FluentValidationSchemaProcessor>();
            document.SchemaProcessors.Add(fluentValidationSchemaProcessor);
        });

        services.AddScoped<FluentValidationSchemaProcessor>();

        services.AddCors(opt =>
            opt.AddPolicy(ApiConstants.CorsPolicy, policy =>
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(origin => true)
                    .AllowCredentials()));

        return services;
    }

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, SecurityHeaderSettings securityHeaderSettings)
    {

        return app.Use(async (context, next) =>
        {
            if (!string.IsNullOrEmpty(securityHeaderSettings.XFrameOptions) && !string.IsNullOrWhiteSpace(securityHeaderSettings.XFrameOptions))
            {
                context.Response.Headers.Add(ApiConstants.XFRAMEOPTIONS, securityHeaderSettings.XFrameOptions);
            }

            if (!string.IsNullOrEmpty(securityHeaderSettings.XContentTypeOptions) && !string.IsNullOrWhiteSpace(securityHeaderSettings.XContentTypeOptions))
            {
                context.Response.Headers.Add(ApiConstants.XCONTENTTYPEOPTIONS, securityHeaderSettings.XContentTypeOptions);
            }

            if (!string.IsNullOrEmpty(securityHeaderSettings.ReferrerPolicy) && !string.IsNullOrWhiteSpace(securityHeaderSettings.ReferrerPolicy))
            {
                context.Response.Headers.Add(ApiConstants.REFERRERPOLICY, securityHeaderSettings.ReferrerPolicy);
            }

            if (!string.IsNullOrEmpty(securityHeaderSettings.PermissionsPolicy) && !string.IsNullOrWhiteSpace(securityHeaderSettings.PermissionsPolicy))
            {
                context.Response.Headers.Add(ApiConstants.PERMISSIONSPOLICY, securityHeaderSettings.PermissionsPolicy);
            }

            if (!string.IsNullOrEmpty(securityHeaderSettings.SameSite) && !string.IsNullOrWhiteSpace(securityHeaderSettings.SameSite))
            {
                context.Response.Headers.Add(ApiConstants.SAMESITE, securityHeaderSettings.SameSite);
            }

            if (!string.IsNullOrEmpty(securityHeaderSettings.XXSSProtection) && !string.IsNullOrWhiteSpace(securityHeaderSettings.XXSSProtection))
            {
                context.Response.Headers.Add(ApiConstants.XXSSPROTECTION, securityHeaderSettings.XXSSProtection);
            }

            await next();
        });
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

    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

        return app;
    }
}
