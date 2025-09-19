using FitHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Business.Dtos
{
    public class ProfileSetupDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string? Goal { get; set; }
        public string Role { get; set; }
        public string? Certification { get; set; }
        public string? Qualifications { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Specialization { get; set; }
    }
}
