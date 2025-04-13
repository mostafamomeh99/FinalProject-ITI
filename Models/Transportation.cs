using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
 public  class Transportation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Driver? Driver { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
