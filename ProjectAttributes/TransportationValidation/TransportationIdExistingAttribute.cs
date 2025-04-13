using DatabaseConnection;
//using Microsoft.Extensions.DependencyInjection;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAttributes.TransportationValidation
{
    public class TransportationIdExistingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dBContext = validationContext.GetService<ApplicationDbContext>();   
            IQueryable<Transportation> transportation = null;
            string transportationid = (string)value;
            transportation = dBContext.Transportations.Where(e => e.Id == transportationid);

            if (!transportation.Any())
                return new ValidationResult(ErrorMessage ?? "transportation not found.");
            return ValidationResult.Success;
        }
    }
}
