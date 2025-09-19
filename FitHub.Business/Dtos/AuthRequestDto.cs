using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Business.Dtos
{
    public class AuthRequestDto
    {
        public string? userName { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
