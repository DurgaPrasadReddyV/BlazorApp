using BlazorApp.Domain.Configurations;

namespace BlazorApp.Host.Extensions
{
    public static class ServiceExtensions
    {
        public static JwtSettings AddJwtSettings(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettingsConfigSection = config.GetSection($"{nameof(JwtSettings)}");
            services.Configure<JwtSettings>(jwtSettingsConfigSection);
            return jwtSettingsConfigSection.Get<JwtSettings>();
        }

        public static ConnectionStrings AddConnectionStrings(this IServiceCollection services, IConfiguration config)
        {
            var connectionStringsConfigSection = config.GetSection($"{nameof(ConnectionStrings)}");
            services.Configure<ConnectionStrings>(connectionStringsConfigSection);
            return connectionStringsConfigSection.Get<ConnectionStrings>();
        }
    }
}
