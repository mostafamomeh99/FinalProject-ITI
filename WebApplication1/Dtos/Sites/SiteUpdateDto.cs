using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.ProjectAttrbuites.GovernmentValidation;


namespace WebApplication1.Dtos.Sites
{
    public class SiteUpdateDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        public string Description { get; set; }

        [Required]
        public int GovernmentId { get; set; }

        public List<SiteImagesGetDto>? SiteImages { get; set; } = new List<SiteImagesGetDto>();

        public List<IFormFile>? Images { get; set; }
    }
}
