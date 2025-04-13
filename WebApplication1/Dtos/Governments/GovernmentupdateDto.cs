using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.ProjectAttrbuites.GovernmentValidation;

namespace WebApplication1.Dtos.Governments
{
      public   class GovernmentupdateDto
    {
        [Required(ErrorMessage = "government is required")]
        public int Id { get; set; }
        [Required(ErrorMessage = "name is required")]
        [MinLength(3, ErrorMessage = "name is too short")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "name is invalid")]
        public string Name { get; set; }
        //[GovernmentExisting] // cause he can update image only

        //[GovernmentImageExisitng]
        public string? ImageUrl { get; set; }
        public  IFormFile? image { get; set; }
    }
}
