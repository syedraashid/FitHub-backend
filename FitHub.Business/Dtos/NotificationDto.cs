using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Business.Dtos
{
    public class NotificationDto
    {
        public string Message { get; set; }
        public string Datetime { get; set; } = DateTime.UtcNow.ToString();
    }
}
