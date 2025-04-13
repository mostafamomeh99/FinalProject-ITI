using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Dtos.Transportations
{
    public class TransportationAddDto
    {

        [Required(ErrorMessage = "Transportation Name is required.")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "name is invalid")]
        public string Name { get; set; }
    }
}
