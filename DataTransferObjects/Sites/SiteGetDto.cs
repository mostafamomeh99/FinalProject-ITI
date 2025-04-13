using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Sites
{
  public  class SiteGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<SiteImagesGetDto> siteImages { get; set; } = new List<SiteImagesGetDto>();
        
    }
}
