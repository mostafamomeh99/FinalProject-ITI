using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class Driver
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Transportation")]
        public string TransportationId { get; set; }
        public Transportation Transportation { get; set; } 
    }
}
