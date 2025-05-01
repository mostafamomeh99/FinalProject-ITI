using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos.Payment
{
    namespace WebApplication1.Dtos.Book
    {
        public class CreateCheckoutSessionDto
        {
            [Required]
            public string BookingId { get; set; }

            [Required]
            public string TripId { get; set; }

            [Range(0.01, double.MaxValue)]
            public decimal Amount { get; set; }

            [Required]
            [Url]
            public string BaseUrl { get; set; }
        }
    }
}
