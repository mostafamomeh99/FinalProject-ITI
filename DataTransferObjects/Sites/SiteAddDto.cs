using Microsoft.AspNetCore.Http;
using ProjectAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Sites
{
   public class SiteAddDto
    {
        [Required(ErrorMessage = "name is required")]
        [MinLength(3, ErrorMessage = "name is too short")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "name is invalid")]
        public string Name { get; set; }
        [Required(ErrorMessage = "description is required")]
        [MinLength(3, ErrorMessage = "description is too short")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z\s]{2,200}[0-9]*$", ErrorMessage = "Description  is invalid")]
        public string Description { get; set; }
        [Required(ErrorMessage = "government is required")]
        [GovernmentIdExisting]
        public int GovernmentId { get; set; }

        [Required]
         public  List<IFormFile> images { get; set; }
    }
}
