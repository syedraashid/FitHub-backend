using FitHub.Infrastructure.Notification;
using FitHub.Infrastructure.Security;
using FitHub.Infrastructure.SignalR;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitHub.Infrastructure
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services , IConfiguration configuration) {
            services.AddScoped<JwtTokenGenerator>();
            services.AddScoped<NotificationService>();
            services.AddHangfire(config =>
            config.UsePostgreSqlStorage(configuration.GetConnectionString("connectionString")));

            services.AddHangfireServer();
            return services;
        }
    }
}
