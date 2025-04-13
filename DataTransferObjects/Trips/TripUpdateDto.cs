using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Trips
{
    public class TripUpdateDto
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
        public bool? IsDeleted { get; set; }
        public bool? OutOfDate { get; set; }
        public List<string> IncludedItems { get; set; } = new List<string>();
        public List<string> ExcludedItems { get; set; } = new List<string>();
        public List<string> Sites { get; set; } = new List<string>();
    }
}
