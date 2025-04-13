using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Accounts
{
    public class RegisteruserDto
    {
        [Required(ErrorMessage = "name is required")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ]{3,15}$", ErrorMessage = "firstname must be only letters with no space")]
        [MaxLength(15, ErrorMessage = "max length of letters is 15")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "name is required")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿ]{3,15}$", ErrorMessage = "lastname must be only letters with no space")]
        [MaxLength(15, ErrorMessage = "max length of letters is 15")]
        public string LastName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@(gmail\.com|yahoo\.com|outlook\.com|icloud\.com)$", ErrorMessage = "not valid email")]
        [MaxLength(254, ErrorMessage = "not valid email")]
        [Required(ErrorMessage = "email  is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Country  is required")]
        [MinLength(3,ErrorMessage = "invalid Country")]
        [MaxLength(56, ErrorMessage = "invalid Country")]
        public string Country { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[^\s]{8,}$", ErrorMessage = "password must contain at least one" +
            " capital , small , special character and numbers ")]
        [MinLength(8, ErrorMessage = "password length is must be at minumum 8")]
        [Required(ErrorMessage = "password is required")]
        public string Password { get; set; }
    }
}
