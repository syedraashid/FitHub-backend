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
                user = new User { OAuthId = payload.Subject, Email = payload.Email, Name = payload.Name };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // 🔹 Generate JWT token
            var token = _tokenGenerator.GenerateJwtToken(user);
            return Ok(new { token, user });
        }
    }
}
