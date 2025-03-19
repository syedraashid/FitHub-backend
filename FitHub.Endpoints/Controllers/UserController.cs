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

namespace FitHub.Endpoints.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly FitHubDbContext _context;

        public UserController(JwtTokenGenerator tokenGenerator , FitHubDbContext context)
        {
            _tokenGenerator = tokenGenerator;
            _context = context;
        }

        [HttpGet("Google-Login")]
        public IActionResult OauthLogin()
        {
            var redirectUrl = Url.Action("OAuthResponse", "User");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("Google-Response")]
        public async Task<IActionResult> OAuthResponse()
        {

            var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authResult.Succeeded)
            {
                return Unauthorized("Google authentication failed.");
            }

            var claims = authResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "User";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
            {
                return Unauthorized("Invalid Google account details.");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    OAuthId = googleId,
                    Name = name,
                    Email = email,
                    Role = UserRoles.Member, 
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }


            // Generate JWT Token
            var token = _tokenGenerator.GenerateJwtToken(user.Id, user.Email, user.Role);

            return Ok(new { Token = token, User = new { user.Id, user.Email, user.Name, user.Role } });

        }
    }
}
