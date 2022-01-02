using BlazorApp.Domain.Configurations;

namespace BlazorApp.Host.Extensions
{
    public static class ServiceExtensions
    {
        public static JwtSettings LoadJwtSettings(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettingsConfigSection = config.GetSection($"{nameof(JwtSettings)}");
            services.Configure<JwtSettings>(jwtSettingsConfigSection);
            return jwtSettingsConfigSection.Get<JwtSettings>();
        }

        public static ConnectionStrings LoadConnectionStrings(this IServiceCollection services, IConfiguration config)
        {
            var connectionStringsConfigSection = config.GetSection($"{nameof(ConnectionStrings)}");
            services.Configure<ConnectionStrings>(connectionStringsConfigSection);
            return connectionStringsConfigSection.Get<ConnectionStrings>();
        }

        public static SwaggerSettings LoadSwaggerSettings(this IServiceCollection services, IConfiguration config)
        {
            var swaggerSettingsConfigSection = config.GetSection($"{nameof(SwaggerSettings)}");
            services.Configure<SwaggerSettings>(swaggerSettingsConfigSection);
            return swaggerSettingsConfigSection.Get<SwaggerSettings>();
        }

        public static CorsSettings LoadCorsSettings(this IServiceCollection services, IConfiguration config)
        {
            var corsSettingsConfigSection = config.GetSection($"{nameof(CorsSettings)}");
            services.Configure<CorsSettings>(corsSettingsConfigSection);
            return corsSettingsConfigSection.Get<CorsSettings>();
        }

        public static SecurityHeaderSettings LoadSecurityHeaderSettings(this IServiceCollection services, IConfiguration config)
        {
            var securityHeaderSettingsConfigSection = config.GetSection($"{nameof(SecurityHeaderSettings)}");
            services.Configure<SecurityHeaderSettings>(securityHeaderSettingsConfigSection);
            return securityHeaderSettingsConfigSection.Get<SecurityHeaderSettings>();
        }

        public static HangfireSettings LoadHangfireSettings(this IServiceCollection services, IConfiguration config)
        {
            var hangfireSettingsConfigSection = config.GetSection($"{nameof(HangfireSettings)}");
            services.Configure<HangfireSettings>(hangfireSettingsConfigSection);
            return hangfireSettingsConfigSection.Get<HangfireSettings>();
        }

        public static MailSettings LoadMailSettings(this IServiceCollection services, IConfiguration config)
        {
            var mailSettingsConfigSection = config.GetSection($"{nameof(MailSettings)}");
            services.Configure<MailSettings>(mailSettingsConfigSection);
            return mailSettingsConfigSection.Get<MailSettings>();
        }
    }
}
