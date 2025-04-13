using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Site
    {
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }

        [ForeignKey("Government")]
        public int GovernmentId { get; set; }
        public Government Government { get; set; }
        public ICollection<SiteImage> SiteImages { get; set; } = new List<SiteImage>();
        public ICollection<TripSites> TripSites { get; set; } = new List<TripSites>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
