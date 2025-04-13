using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Drivers
{
    public class DriverGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TransportationId { get; set; }
        public string TransportationName { get; set; } 
    }
}
