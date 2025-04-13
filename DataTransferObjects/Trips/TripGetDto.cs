using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Trips
{
    public class TripGetDto
    {
        public string TripId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public decimal Money { get; set; }
        public int AvailablePeople { get; set; }
        public double TripRating { get; set; }
        public int UserNumbersRating { get; set; }
        public int RatingValue { get; set; } = 5;
        public int MaxPeople { get; set; }
        public bool IsDeleted { get; set; }
        public bool OutOfDate { get; set; }
    }
}
