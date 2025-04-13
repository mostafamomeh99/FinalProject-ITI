using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Rating
    {
        [ForeignKey("Trip")]
        public string TripId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public int UserRate { get; set; }
        public  int RateNumber  { get; set; }= 5;
        public Trip Trip { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
