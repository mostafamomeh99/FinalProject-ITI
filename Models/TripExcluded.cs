using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public  class TripExcluded
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Trip")]
        public string TripId { get; set; }
        public string Item { get; set; }
        public Trip Trip { get; set; }
    }
}
