using FitHub.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Business.Dtos
{
    public class AuthResponseDto
    {
        public User user { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
