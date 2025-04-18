using FitHub.Business.Dtos;
using FitHub.Business.IRepository;
using FitHub.Domain.DataBase;
using FitHub.Domain.Enums;
using FitHub.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FitHub.Infrastructure.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly FitHubDbContext _fitHubDbContext;

        public UserRepo(FitHubDbContext fitHubDbContext)
        {
            _fitHubDbContext = fitHubDbContext;   
        }

        public async Task<string> CompleteProfile(ProfileSetupDto profile)
        {

            try
            {
                var user = await this.GetByEmail(profile.Email);
                if (profile.Role == UserRoles.Member.ToString())
                {
                    var member = new Member
                    {
                        FullName = profile.FullName,
                        UserId = user.Id,
                        Age = profile.Age,
                        Weight = profile.Weight,
                        Height = profile.Height,
                        Goal = Enum.TryParse<Purpose>(profile.Goal, out Purpose goal) ? goal : Purpose.Maintenance,
                        User = user
                    };
                    user.Member = member;
                    await _fitHubDbContext.Members.AddAsync(member);
                }
                else if (profile.Role == UserRoles.Nuritionist.ToString())
                {
                    var nutritionist = new Nutritionist
                    {
                        FullName = profile.FullName,
                        UserId = user.Id,
                        Certification = profile.Certification ?? string.Empty,
                        Qualifications = profile.Qualifications ?? string.Empty,
                        YearsOfExperience = profile.YearsOfExperience ?? 0,
                        Specialization = profile.Specialization ?? string.Empty,
                        User = user
                    };
                    user.Nutritionist = nutritionist;
                    await _fitHubDbContext.Nutritionists.AddAsync(nutritionist);
                }
                else if (profile.Role == UserRoles.Trainer.ToString())
                {
                    var trainer = new Trainer
                    {
                        FullName = profile.FullName,
                        UserId = user.Id,
                        Certifications = profile.Certification ?? string.Empty,
                        YearsOfExperience = profile.YearsOfExperience ?? 0,
                        Specialization = profile.Specialization ?? string.Empty,
                        User = user
                    };
                    user.Trainer = trainer;
                    await _fitHubDbContext.Trainers.AddAsync(trainer);
                }
                user.Role = Enum.TryParse<UserRoles>(profile.Role, out UserRoles role) ? role : UserRoles.Member;
                user.IsProfileSetupComplete = true;
                await _fitHubDbContext.SaveChangesAsync();
                return "Created SuccessFully";
            }
            catch (Exception ex)
            {
                return $"Failed Creation {ex.Message}";
            }
        }

        public async Task<User> GetByEmail(string mail)
        {
            return await _fitHubDbContext.Users.FirstOrDefaultAsync(_ => _.Email == mail);
        }
    }
}
