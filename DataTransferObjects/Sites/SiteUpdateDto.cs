using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProjectAttributes;

namespace DataTransferObjects.Sites
{
   public class SiteUpdateDto
    {
        [Required(ErrorMessage = "id is required")]
        public string Id { set; get; }
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
        public List<SiteImagesGetDto>? siteImages { get; set; } = new List<SiteImagesGetDto>();

         public  List<IFormFile>? images { get; set; }
    }
}
