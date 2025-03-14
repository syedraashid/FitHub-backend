using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Domain.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int UserId { get; set; }
        public string Certifications { get; set; }
        public int YearsOfExperience { get; set; }
        public string Specialization { get; set; }
        public User User { get; set; }
        public ICollection<Member>? Members { get; set; } = new List<Member>();
    }
}
