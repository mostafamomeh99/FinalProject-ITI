namespace WebApplication1.Dtos.Book
{
    public class BookUserDto
    {
        public string BookId { get; set; }
        public string TripId { get; set; }
        public string TripName { get; set; }
        public DateTime DateBook { get; set; }
        public DateTime StartComingDate { get; set; }
        public DateTime EndComingDate { get; set; }
        public int NumberDays { get; set; }
        public int NumberPeople { get; set; }
        public decimal AmountMoney { get; set; }
        public string Status { get; set; }  // You might want to add booking status
    }
}
