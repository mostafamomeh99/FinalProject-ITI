using Microsoft.AspNetCore.Identity;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositoryService
{
    public interface IAccountRepository : IEmailService , IOtpService
    {
        Task<ApplicationUser> FindUserByIdAsync(string userId);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<ApplicationUser> FindUserByEmailAsync(string userId);
        Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password);
        Task<string> GenerateToken(ApplicationUser userLog, string role);
        Task AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser applicationUser, string token, string newpassowrd);
        bool CheckUniqueEmail(string email);
        string CreateUserName(string FirstName, string LastName);
    }
}
