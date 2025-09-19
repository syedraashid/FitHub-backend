using FitHub.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitHub.Business.Interfaces
{
    public interface IUserServices
    {
        public Task<string> CompleteProfileSetup(ProfileSetupDto profile);
    }
}
