using FitHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Domain.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? OAuthId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public UserRoles Role { get; set; }
        public bool IsProfileSetupComplete { get; set; }
        //Navigation Property
        public Nutritionist? Nutritionist { get; set; }
        public Trainer? Trainer { get; set; }
        public Member? Member { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
