using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public  class TripSites
    {
        public string TripId { get; set; }
        public Trip Trip { get; set; }
        public string SiteId { get; set; }
        public Site Site { get; set; }
        public string PlanInSite { get; set; }
        public int NumberDays { get; set; }
    }
}
