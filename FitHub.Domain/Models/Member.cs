using FitHub.Domain.Common;
using FitHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Domain.Models
{
    public class Member:AuditableEntity
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int UserId { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public Purpose Goal {  get; set; }
        public int? NutritionistId { get; set; }
        public int? TrainerId { get; set; }
        public Nutritionist? Nutritionist { get; set; }
        public User User { get; set; }
        public Trainer? Trainer { get; set; }
    }
}
