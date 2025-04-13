using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.StoredprocMapping
{
   public class TripSiteDetailDto
    {
        public string TripId { get; set; }
        public string TripName { get; set; }
        public string TripDescription { get; set; }
        public bool TripIsDeleted { get; set; }
        public int AvaiblePeople { get; set; }
        public int TripTotalDuration { get; set; }
        public DateTime TripendDate { get; set; }
        public DateTime TripStartDate { get; set; }
        public int TripMaxPeople { get; set; }
        public decimal TripMoney { get; set; }
        public bool TripisOutOfDate { get; set; }
        public string TransportId { get; set; }
        public string TransportName { get; set; }
        public int IncludedId { get; set; }
        public string IncludedItem { get; set; }
        public int ExcludedId { get; set; }
        public string ExcludedItem { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteDescription { get; set; }
        public int ImagId { get; set; }
        public string SiteImageName { get; set; }
        public string PlanOfTripInSite { get; set; }
        public int GovernmentId { get; set; }
        public string GovernmentName { get; set; }
        public string GovernmentImage { get; set; }
        public int NumberOfDaysInSite { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
    }
}
