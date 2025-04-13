using System.ComponentModel.DataAnnotations;

namespace DataTransferObjects.Account
{
    public class AccountLoginDto
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|icloud\.com)$", ErrorMessage = "not valid email")]
        [MaxLength(254, ErrorMessage = "mot valid email")]
        [Required(ErrorMessage = "email  is required")]
        public string Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[^\s]{8,}$", ErrorMessage = "password must contain at least one" +
          " capital , small , special character and numbers ")]
        [MinLength(8, ErrorMessage = "password length is must be at minumum 8")]
        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }
    }
}
