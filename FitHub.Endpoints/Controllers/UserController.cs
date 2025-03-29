using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FitHub.Domain.Enums;
using FitHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FitHub.Domain.DataBase;
using FitHub.Infrastructure.Security;
using Newtonsoft.Json;
using Google.Apis.Auth;
using FitHub.Business.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace FitHub.Endpoints.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly FitHubDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(JwtTokenGenerator tokenGenerator , FitHubDbContext context, IConfiguration configurations)
        {
            _tokenGenerator = tokenGenerator;
            _context = context;
            _configuration = configurations;
        }

        [HttpPost("Google-Login")]
        public async Task<IActionResult> OauthLogin(GoogleRequestDto request)
        {
            var client = new HttpClient();

            // 🔹 Exchange code for access_token and id_token
            var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", _configuration["Google:ClientId"] },
                    { "client_secret", _configuration["Google:ClientSecret"] },
                    { "code", request.code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", "http://localhost:3000/auth/callback" }
                }));

            if (!tokenResponse.IsSuccessStatusCode) return Unauthorized("Invalid Google Code");

            var tokenData = JsonConvert.DeserializeObject<GoogleResponseDto>(await tokenResponse.Content.ReadAsStringAsync());

            // 🔹 Verify Google Token
            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenData.IdToken);
            if (payload == null) return Unauthorized("Invalid Google Token");

            var user = _context.Users.FirstOrDefault(user => user.Email == payload.Email);

            if (user == null)
            {
                user = new User { OAuthId = payload.Subject, Email = payload.Email, Name = payload.Name, RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7) };
                _context.Users.Add(user);
            }

            // 🔹 Generate JWT token
            var accessToken = _tokenGenerator.GenerateJwtAccessToken(user);
            var refreshToken = _tokenGenerator.GenerateJwtAccessToken(user);

            user.RefreshToken = refreshToken;
            await _context.SaveChangesAsync();
            return Ok(new AuthResponseDto{ user = user, accessToken = accessToken ,refreshToken = refreshToken});
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshAccessToken(string RefershToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            try
            {
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false // Allow expired tokens to be validated
                };

                var principal = handler.ValidateToken(RefershToken, validationParams, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null)
                    return Unauthorized(new { message = "Invalid refresh token" });

                var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var user = await _context.Users.FindAsync(userId);

                if (user == null || user.RefreshToken != RefershToken)
                    return Unauthorized(new { message = "Invalid refresh token" });

                if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    await _context.SaveChangesAsync();
                    return Unauthorized(new { message = "Refresh token expired" });
                }

                // Generate new tokens
                var newAccessToken = _tokenGenerator.GenerateJwtAccessToken(user);
                var newRefreshToken = _tokenGenerator.GenerateJwtRefreshToken(user);

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    accessToken = newAccessToken,
                    refreshToken = newRefreshToken
                });
            }
            catch
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

        }
    }
}
