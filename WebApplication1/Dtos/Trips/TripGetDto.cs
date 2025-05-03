using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Dtos.Sites;

namespace WebApplication1.Dtos.Trips
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
        public int MaxPeople { get; set; }
        public List<string> IncludedItems { get; set; } = new List<string>();
        public List<string> ExcludedItems { get; set; } = new List<string>();
        public List<string> Sites { get; set; } = new List<string>();
        public List<TripImageDto> TripImages { get; set; } = new List<TripImageDto>();

    }
}
