using DatabaseConnection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAttrubitesTry.TransportationValidation
{
    public class TransportationNotTakenAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dBContext = validationContext.GetService<ApplicationDbContext>();
            IQueryable<Driver>? transportation = null;
            string transportationid = (string)value;
            transportation = dBContext.Drivers.Where(e=>e.TransportationId==transportationid);

            if (!transportation.Any())
                return new ValidationResult(ErrorMessage ?? "transportation is already with another driver.");
            return ValidationResult.Success;
        }
    }
}
