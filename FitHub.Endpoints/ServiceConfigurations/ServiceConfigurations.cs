using FitHub.Business.Interfaces;
using FitHub.Business.IRepository;
using FitHub.Business.Services;
using FitHub.Domain.DataBase;
using FitHub.Infrastructure;
using FitHub.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            }).AddJwtBearer(options =>
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
                  options.Events = new JwtBearerEvents
                  {
                      OnMessageReceived = context =>
                      {
                          var accessToken = context.Request.Query["access_token"];

                          // If the request is for our SignalR hub...
                          var path = context.HttpContext.Request.Path;
                          if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                          {
                              // Read the token from the query string
                              context.Token = accessToken;
                          }

                          return Task.CompletedTask;
                      }
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
            services.AddSignalR();
            services.AddInfrastructureServices(configuration);
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserServices, UserServices>();

            return services;
        }
    }
}
