using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Domain.Enums
{
    public enum Purpose
    {
        WeightLoss = 1000,
        MuscleGain,
        Maintenance,
        GeneralHealth,
        Endurance,
        Strength,
        Flexibility,
        testing
    }
    public enum UserRoles
    {
        Admin=1,
        Member,
        Nuritionist,
        Trainer,
        Tester
    }
}
