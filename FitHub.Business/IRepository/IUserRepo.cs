using FitHub.Domain.Models;
using FitHub.Business.Dtos;

namespace FitHub.Business.IRepository
{
    public interface IUserRepo
    {
        Task<User> GetByEmail(string mail);
        Task<string> CompleteProfile(ProfileSetupDto profile);
    }
}
