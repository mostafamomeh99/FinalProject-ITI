using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos.Book
{
    public class UpdateBookDto
    {
        [Required]
        public string TripName { get; set; }


        [DataType(DataType.Date)]
        public DateTime StartComingDate { get; set; }


        [DataType(DataType.Date)]
        public DateTime EndComingDate { get; set; }


        [Range(1, int.MaxValue)]
        public int NumberPeople { get; set; }

        public int? NumberDays { get; set; }
        public decimal? AmountMoney { get; set; }
    }
}
