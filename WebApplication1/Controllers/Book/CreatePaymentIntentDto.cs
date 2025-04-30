using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos.Book
{
    namespace WebApplication1.Dtos.Book
    {
        public class CreatePaymentIntentDto
        {
            [Required]
            public string BookingId { get; set; }

            [Required]
            public string TripId { get; set; }

            [Required]
            [Range(0.01, double.MaxValue)]
            public decimal Amount { get; set; }

            [Required]
            [EmailAddress]
            public string CustomerEmail { get; set; }
        }
    }

}
