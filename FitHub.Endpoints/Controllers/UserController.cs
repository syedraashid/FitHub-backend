using Microsoft.AspNetCore.Mvc;
using FitHub.Domain.Models;
using FitHub.Domain.DataBase;
using FitHub.Infrastructure.Security;
using Newtonsoft.Json;
using Google.Apis.Auth;
using FitHub.Business.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Endpoints.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly FitHubDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<object> _passwordHasher = new();

        public UserController(JwtTokenGenerator tokenGenerator, FitHubDbContext context, IConfiguration configurations)
        {
            _tokenGenerator = tokenGenerator;
            _context = context;
            _configuration = configurations;
        }

        [HttpPost("Google-Login")]
        public async Task<IActionResult> OauthLogin(GoogleRequestDto request)
        {
            var client = new HttpClient();

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

            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenData.IdToken);
            if (payload == null) return Unauthorized("Invalid Google Token");

            var user = _context.Users.FirstOrDefault(user => user.Email == payload.Email);

            if (user == null)
            {
                user = new User { OAuthId = payload.Subject, Email = payload.Email, Name = payload.Name, RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7) };
                _context.Users.Add(user);
            }

            var accessToken = _tokenGenerator.GenerateJwtAccessToken(user);
            var refreshToken = _tokenGenerator.GenerateJwtAccessToken(user);

            user.RefreshToken = refreshToken;
            await _context.SaveChangesAsync();
            return Ok(new AuthResponseDto { user = user, accessToken = accessToken, refreshToken = refreshToken });
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshAccessToken([FromBody]string RefershToken)
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
                    ValidateLifetime = false
                };

                var principal = handler.ValidateToken(RefershToken, validationParams, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken)
                    return Unauthorized(new { message = "Invalid refresh token" });

                var user = await _context.Users.FirstOrDefaultAsync(_ => _.RefreshToken == RefershToken);

                if (user == null || user.RefreshToken != RefershToken)
                    return Unauthorized(new { message = "Invalid refresh token" });

                if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                    return Unauthorized(new { message = "Refresh token expired" });

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

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthRequestDto request)
        {
            var User = _context.Users.FirstOrDefault(_ => _.Email == request.Email);

            if (User == null) return Unauthorized("User not Found");

            var ValidateResult = _passwordHasher.VerifyHashedPassword(null, User.Password, request.Password);
            bool IsvalidUser = ValidateResult == PasswordVerificationResult.Success;

            if (!IsvalidUser) return Unauthorized("Password Doesn't Match");

            return Ok(new AuthResponseDto
            {
                accessToken = _tokenGenerator.GenerateJwtAccessToken(User),
                refreshToken = _tokenGenerator.GenerateJwtRefreshToken(User),
                user = User
            });
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(AuthRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Password)) return BadRequest("Improper Request");

            var IsUserExists = _context.Users.FirstOrDefault(_ => _.Email == request.Email);

            if (IsUserExists != null) return BadRequest("Mail ID ALready Exist");

            var User = new User
            {
                Name = request.userName ?? "" ,
                Email = request.Email,
                Password = _passwordHasher.HashPassword(null ,request.Password),
            };

            await _context.Users.AddAsync(User);
            var refreshToken = _tokenGenerator.GenerateJwtRefreshToken(User);
            User.RefreshToken = refreshToken;
            User.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return Ok(new AuthResponseDto
            {
                accessToken = _tokenGenerator.GenerateJwtAccessToken(User),
                refreshToken = refreshToken,
                user = User
            });
        }
    }
}
