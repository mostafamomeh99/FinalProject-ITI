using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Accounts
{
    public class ResetPasswordDto
    {

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[^\s]{8,}$", ErrorMessage = "password must contain at least one" +
          " capital , small , special character and numbers ")]
        [MinLength(8, ErrorMessage = "password length is must be at minumum 8")]
        [Required(ErrorMessage = "password is required")]
        public string NewPassword { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|icloud\.com)$", ErrorMessage = "not valid email")]
        [MaxLength(254, ErrorMessage = "not valid email")]
        [Required(ErrorMessage = "email  is required")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"\d{6}", ErrorMessage = "invalid otp")]
        public string OTP { get; set; }
    }
}
