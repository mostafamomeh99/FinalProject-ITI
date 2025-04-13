using DatabaseConnection;
using IRepositoryService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryFactory
{
    public class AccountRepository : IAccountRepository 
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly IOtpService otpService;
        public AccountRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext
    , IConfiguration configuration , IEmailService emailService , IOtpService otpService
          )
        {

            this.userManager = userManager;
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.emailService = emailService;
            this.otpService = otpService;
        }

        public async Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }

        public async Task<ApplicationUser> FindUserByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }
        public async Task<ApplicationUser> FindUserByEmailAsync(string userId)
        {
            return await userManager.FindByEmailAsync(userId);
        }
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await userManager.GetRolesAsync(user);
        }
        public async Task AddToRoleAsync(ApplicationUser user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await userManager.CheckPasswordAsync(user, password);
        }

        public bool CheckUniqueEmail(string email)
        {
            var ExistingEmail = dbContext.Users.Where(e => e.Email == email).FirstOrDefault();

            if (ExistingEmail != null)
            {
                return false;
            }
            return true;
        }

        public string CreateUserName(string FirstName, string LastName)
        {
            string UserName = FirstName + LastName + Guid.NewGuid().ToString("N");

            return UserName;

        }

        public string GenerateOtp(ApplicationUser user)
        {
            string otp = otpService.GenerateOtp(user);
            return otp;
        }

        public async Task<bool> ValidateOtp(ApplicationUser user, string userOtp, string newpassword)
        {
           return  await otpService.ValidateOtp(user, userOtp, newpassword);

        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await emailService.SendEmailAsync(email, subject, message);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser applicationUser, string token, string newpassowrd)
        {

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            applicationUser.PasswordHash = passwordHasher.HashPassword(applicationUser, newpassowrd);
            var updateResult = await userManager.UpdateAsync(applicationUser);
            return updateResult;
        }

        public async Task<string> GenerateToken(ApplicationUser userLog, string role)
        {
            var secretKeys = new List<string>();
            configuration.GetSection("Jwt:SecretKeys").Bind(secretKeys);
            var selectedKey = secretKeys[new Random().Next(secretKeys.Count)];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(selectedKey));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userLog.Id),
            new Claim(JwtRegisteredClaimNames.Name, userLog.UserName),
            new Claim(JwtRegisteredClaimNames.Email, userLog.Email),
            new Claim(ClaimTypes.MobilePhone, userLog.PhoneNumber??"not set"),
            new Claim("Country", userLog.PhoneNumber??"not set"),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
             issuer: "https://localhost:7028",
             claims: claims,
             expires: DateTime.UtcNow.AddDays(1),
             signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
         );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }


    }
    }
