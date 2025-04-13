using Microsoft.AspNetCore.Identity;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositoryService
{
  public  interface IOtpService
    {
        string GenerateOtp(ApplicationUser user);
       Task<bool> ValidateOtp(ApplicationUser user, string userOtp, string newpassword);
    }
}
