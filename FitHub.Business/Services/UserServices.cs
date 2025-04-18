using FitHub.Business.Dtos;
using FitHub.Business.Interfaces;
using FitHub.Business.IRepository;

namespace FitHub.Business.Services
{
    public class UserServices:IUserServices
    {
        private readonly IUserRepo _userRepo;
        public UserServices(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<string> CompleteProfileSetup(ProfileSetupDto profile)
        {
            if (profile != null)
            {
                return await _userRepo.CompleteProfile(profile);
            }

            return "Profile Can't be Empty";
        }
    }
}
