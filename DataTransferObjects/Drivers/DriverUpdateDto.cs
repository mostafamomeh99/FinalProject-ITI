using ProjectAttributes.TransportationValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObjects.Drivers
{
    public class DriverUpdateDto
    {
        [Required(ErrorMessage = "Driver ID is required.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Driver Name is required.")]
        [RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "name is invalid")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Transportation is required.")]
        [TransportationIdExisting]
        public string TransportationId { get; set; }
    }
}
