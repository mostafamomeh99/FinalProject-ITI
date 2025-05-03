using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class TripImage
    {

        [Key]
        public int Id { get; set; }
        [ForeignKey("Site")]
        public string TripId { get; set; }
        public string ImageName { get; set; }

        public Trip Trip { get; set; }
    }
}
