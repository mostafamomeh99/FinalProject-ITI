using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class SiteImage
    {
        
        [Key]
        public int Id { get; set; }
        [ForeignKey("Site")]
        public string SiteId { get; set; }
        public string ImageName { get; set; }

        public Site Site { get; set; }
    }
}
