using FitHub.Business.Interfaces;
using FitHub.Business.IRepository;
using FitHub.Business.Services;
using FitHub.Domain.DataBase;
using FitHub.Infrastructure;
using FitHub.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FitHub.Endpoints.ServiceConfigurations
{
    public static class ServiceConfigurations
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //DbContext Section
            services.AddDbContext<FitHubDbContext>(_ => _.UseNpgsql(configuration.GetConnectionString("connectionString")));

            //AuthenticationSection
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddCookie()
                .AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]);
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"]
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder => builder.WithOrigins("http://localhost:3000")
                                     .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials());
            });

            services.AddAuthorization();
            services.AddInfrastructureServices(configuration);
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserServices, UserServices>();

            return services;
        }
    }
}
