using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Dtos.Trips   
{
    public class TripAddDto
    {
        public string TripId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public decimal Money { get; set; }
        public int AvailablePeople { get; set; }
        public int MaxPeople { get; set; }

        public int RatingValue { get; set; }
        public int RatingNumber { get; set; } = 5;
        public List<string> SitesImages { get; set; } = new List<string>();
    }
}
