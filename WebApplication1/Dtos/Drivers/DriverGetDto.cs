using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Dtos.Drivers
{
    public class DriverGetDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TransportationId { get; set; }
        public string TransportationName { get; set; } 
    }
}
