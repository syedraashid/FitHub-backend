using FitHub.Infrastructure.Security;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services , IConfiguration configuration) {
            services.AddScoped<JwtTokenGenerator>();
            services.AddHangfire(config =>
            config.UsePostgreSqlStorage(configuration.GetConnectionString("connectionString")));

            services.AddHangfireServer();
            return services;
        }
    }
}
