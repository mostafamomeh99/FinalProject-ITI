using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class Book
    {
        [Key]
        public string BookId { get; set; }
        [ForeignKey("Trip")]
        public string TripId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public DateTime? DateBook { get; set; }
        public DateTime StartComingDate { get; set; }
        public DateTime EndComingDate { get; set; }
        public int NumberDays { get; set; }
        public int NumberPeople { get; set; }
        public decimal AmountMoney { get; set; }
        public Trip Trip { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
