using IRepositoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using WebApplication1.Dtos.Accounts;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUnitOfWork unitOFWork;

        public AccountController(IUnitOfWork unitOFWork)
        {
            this.unitOFWork = unitOFWork;
        }

        [HttpGet("claims")]
        [Authorize(Roles ="User")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(RegisteruserDto newaccout)
        {
            if (ModelState.IsValid)
            {
                var checkemail = unitOFWork.Accounts.CheckUniqueEmail(newaccout.Email);
                if (checkemail)
                {
                    ApplicationUser applicationUser = new ApplicationUser()
                    {
                        UserName = unitOFWork.Accounts.CreateUserName(newaccout.FirstName, newaccout.LastName),
                        Email = newaccout.Email
                    };

                    var result = await unitOFWork.Accounts.RegisterUserAsync(applicationUser, newaccout.Password);
                    if (result.Succeeded)
                    {

                        await unitOFWork.Accounts.AddToRoleAsync(applicationUser, "User");

                        return Ok(new { response = new List<string> { "regstrion success" } });
                    }

                    else
                    {
                        List<string> errorslist = new List<string>();
                        foreach (var error in result.Errors)
                        { errorslist.Add(error.Description); }
                        return BadRequest(new { errors = errorslist });
                    }
                }
                else
                {
                    return BadRequest(new { errors = new List<string> { "There is already an account with this email." } });
                }

            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RregisterAdmin(RegisteruserDto newaccout)
        {
            if (ModelState.IsValid)
            {
                var checkemail = unitOFWork.Accounts.CheckUniqueEmail(newaccout.Email);
                if (checkemail)
                {
                    ApplicationUser applicationUser = new ApplicationUser()
                    {
                        UserName = unitOFWork.Accounts.CreateUserName(newaccout.FirstName, newaccout.LastName),
                        Email = newaccout.Email
                    };

                    var result = await unitOFWork.Accounts.RegisterUserAsync(applicationUser, newaccout.Password);
                    if (result.Succeeded)
                    {

                        await unitOFWork.Accounts.AddToRoleAsync(applicationUser, "Admin");

                        return Ok(new { response = new List<string> { "regstrion success" } });
                    }

                    else
                    {
                        List<string> errorslist = new List<string>();
                        foreach (var error in result.Errors)
                        { errorslist.Add(error.Description); }
                        return BadRequest(new { errors = errorslist });
                    }
                }
                else
                {
                    return BadRequest(new { errors = new List<string> { "There is already an account with this email." } });
                }

            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> login(AccountLoginDto newaccout)
        {
            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                ApplicationUser user = await unitOFWork.Accounts.FindUserByEmailAsync(newaccout.Email);
                if (user != null)
                {
                    var result = await unitOFWork.Accounts.CheckPasswordAsync(user, newaccout.Password);
                    if (result)
                    {
                        var roles = await unitOFWork.Accounts.GetUserRolesAsync(user);

                        string token = await unitOFWork.Accounts.GenerateToken(user, roles[0]);

                        return Ok(new
                        {
                            Token = token
                        });
                    }
                    else
                    {
                        return BadRequest(new { errors = new List<string> { "invalid information" } });
                    }

                }
                else
                {
                    return BadRequest(new { errors = new List<string> { "there is no account with this email" } });
                }

            }
            else
            {
                return BadRequest(new { errors = new List<string> { "invalid information" } });
            }
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await unitOFWork.Accounts.FindUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest(new { errors = new List<string> { "There is no account associated with this email." } });
                }
                //var roles = await unitOFWork.Accounts.GetUserRolesAsync(user);

                var Otp = unitOFWork.Accounts.GenerateOtp(user);

                // Send the reset email
                string htmlBody = $@"
<html>
    <body style='font-family: Arial, sans-serif; color: #333; padding: 20px; background-color: #f5f5f5;'>
        <div style='background-color: #ffffff; padding: 30px; border-radius: 8px; max-width: 500px; margin: 0 auto; text-align: center; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
            <h1 style='color: #4CAF50;'>Your OTP Code</h1>
            <p style='font-size: 16px; line-height: 1.5;'>
                Hello,<br />
                You recently requested to reset your password. Use the following One-Time Password (OTP) to proceed:
            </p>
            <div style='margin: 20px 0; font-size: 32px; font-weight: bold; color: #4CAF50;'>{Otp}</div>
            <p style='font-size: 14px; color: #777;'>
                This code is valid for a limited time. Do not share it with anyone.
            </p>
            <p style='font-size: 14px; color: #777; margin-top: 30px;'>
                If you did not request this, you can safely ignore this email.
            </p>
        </div>
    </body>
</html>";

                await unitOFWork.Accounts.SendEmailAsync(user.Email, "OTP Request", htmlBody);

                return Ok(new { message = "An OTP Code has been sent to your email." });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await unitOFWork.Accounts.FindUserByEmailAsync(model.Email);

                if (user == null)
                {
                    return BadRequest(new { errors = new List<string> { "Invalid email." } });
                }

                var isvalid = await unitOFWork.Accounts.ValidateOtp(user, model.OTP, model.NewPassword);
                if (isvalid) { 
                return Ok(new { message = "Your password has been reset successfully." });
                }
                else
                {
                    return BadRequest(new { errors = new List<string> { "Invalid process." } });
                }
            }
            else
            {
                return BadRequest(new { errors = new List<string> { "Invalid process." } });
            }
        }

    }
}
