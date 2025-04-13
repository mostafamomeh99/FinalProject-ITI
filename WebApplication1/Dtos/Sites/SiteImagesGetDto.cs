using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Dtos.Sites
{
   public class SiteImagesGetDto
    {
        public int Id { get; set; }
        public string SiteId { get; set; }
        public string  Image { get; set; }
    }
}
