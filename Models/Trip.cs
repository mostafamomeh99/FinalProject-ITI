using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Trip
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
        public bool? IsDeleted { get; set; } = false;
        public bool? OutOfDate { get; set; } = false;
        [ForeignKey("Transportation")]
        public string? TransportationId { get; set; } 
        public ICollection<TripIncluded> IncludedItems { get; set; } = new List<TripIncluded>();
        public ICollection<TripExcluded> ExcludedItems { get; set; }= new List<TripExcluded>();
        public ICollection<Site> Sites { get; set; } = new List<Site>();

        public ICollection<TripImage> TripImages { get; set; } = new List<TripImage>();
       
        public ICollection<TripSites> TripSites { get; set; } = new List<TripSites>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public Transportation Transportation { get; set; }
        public ICollection<ApplicationUser>? ApplicationUsers { get; set; } = new List<ApplicationUser>();
    }
}
