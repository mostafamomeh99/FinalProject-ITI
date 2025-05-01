using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos.Book
{
    public class BookDto
    {
        public string BookId { get; set; }
        public string TripId { get; set; }
        public string ApplicationUserId { get; set; }
        public DateTime? DateBook { get; set; }
        public DateTime StartComingDate { get; set; }
        public DateTime EndComingDate { get; set; }
        public int NumberDays { get; set; }
        public int NumberPeople { get; set; }
        public decimal AmountMoney { get; set; }
    }

}
