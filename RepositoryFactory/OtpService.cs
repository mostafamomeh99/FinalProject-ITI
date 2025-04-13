using IRepositoryService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryFactory
{
  public  class OtpService : IOtpService
    {
        private readonly UserManager<ApplicationUser> userManager;
        public OtpService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public  string GenerateOtp(ApplicationUser user)
        {
            byte[] secretBytes = Guid.Parse(user.ConcurrencyStamp).ToByteArray();
            Console.WriteLine(string.Join(',', secretBytes));
            // need to encoduing base32 that is upper case , 2-5
            // Guid is like 795b7e76-067a-4dbf-82dc-8cad9221cfea , 128 bit
            // so first convert to byte 16 byte 87,226,178,76,240,211,108,65,147,225,91,2,123,91,44,14
            var totp = new Totp(secretBytes, step: 1);
            var otp = totp.ComputeTotp();
            Console.WriteLine("generated otp"+otp);
            return otp;
        }

        public  async Task<bool> ValidateOtp(ApplicationUser user, string userOtp, string newpassword)
        {
            byte[] secretBytes = Guid.Parse(user.ConcurrencyStamp).ToByteArray();
            var totp = new Totp(secretBytes, 1);
            Console.WriteLine("Expected OTP: " + totp.ComputeTotp());

            Console.WriteLine(totp.ComputeTotp());

            bool isValid = totp.VerifyTotp(userOtp, out long timeStepMatched
                , new VerificationWindow(previous: 240, future: 240));
            Console.WriteLine(timeStepMatched);
            Console.WriteLine(isValid);
            if (isValid)
            {
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = passwordHasher.HashPassword(user, newpassword);
                var updateResult = await userManager.UpdateAsync(user);
                return true;
            }
            return false;
        }
    }
}
