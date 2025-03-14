using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Domain.Common
{
    public class AuditableEntity
    {
        public int CreateBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
