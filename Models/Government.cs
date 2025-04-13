using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public  class Government
    {
        public int GovernmentId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public ICollection<Site> Sites { get; set; } = new List<Site>();

    }
}
