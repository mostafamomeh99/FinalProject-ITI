using DatabaseConnection;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.ProjectAttrbuites.GovernmentValidation
{
   public class GovernmentIdExistingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dBContext = validationContext.GetService<ApplicationDbContext>();
            Government government = null;
            int id = (int)value;
            government = dBContext.Governments.Find(id);

            if (government == null)
                return new ValidationResult(ErrorMessage ?? "Government not found.");
            return ValidationResult.Success;
        }
    }
}
