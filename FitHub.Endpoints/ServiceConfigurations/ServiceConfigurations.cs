using FitHub.Domain.DataBase;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Endpoints.ServiceConfigurations
{
    public static class ServiceConfigurations
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration) {

            services.AddDbContext<FitHubDbContext>(_ => _.UseNpgsql(configuration.GetConnectionString("connectionString")));
        }
    }
}
