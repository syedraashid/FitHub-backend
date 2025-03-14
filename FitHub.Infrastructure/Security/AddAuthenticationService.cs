using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FitHub.Infrastructure.Security
{
    public static class AddAuthenticationService
    {
        public static void AuthenticationService(this IServiceCollection service, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("jwtsettings");

            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = Encoding.UTF8.GetBytes(jwtSettings["Secretkey"]);
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = jwtSettings["JwtSettings:Issuer"],
                    ValidAudience = jwtSettings["JwtSettings:Audience"]
                };
            }).AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["GoogleAuth:ClientId"];
                googleOptions.ClientSecret = configuration["GoogleAuth:ClientSecret"];
                googleOptions.CallbackPath = "/signin-google";
            });

        }
    }
}
