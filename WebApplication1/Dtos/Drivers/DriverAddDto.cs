using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.ProjectAttrbuites.TransportationValidation;

namespace WebApplication1.Dtos.Drivers
{
    public class DriverAddDto
    {

        [Required(ErrorMessage = "Driver Name is required.")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "name is invalid")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Transportation is required.")]
        //[TransportationIdExisting]
        //[TransportationNotTaken]
        public string TransportationId { get; set; }
    }
}
