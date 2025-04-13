using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
  public  class ApplicationUser : IdentityUser
    {
        public string? Country { get; set; }
        public int? Age { get; set; }
        public ICollection<Trip>? Trips { get; set; } = new List<Trip>();
        public ICollection<Book>? Bookings { get; set; } = new List<Book>();

        public ICollection<Rating>? Ratings { get; set; } = new List<Rating>();

    }
}
