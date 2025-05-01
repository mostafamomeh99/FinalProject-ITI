using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos.Book
{
    public class UpdateBookDto
    {
        public DateTime StartComingDate { get; set; }
        public DateTime EndComingDate { get; set; }
        public int NumberPeople { get; set; }
    }
}
