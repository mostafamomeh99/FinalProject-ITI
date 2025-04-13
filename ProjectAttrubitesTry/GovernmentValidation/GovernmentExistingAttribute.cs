using DatabaseConnection;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System.ComponentModel.DataAnnotations;

namespace ProjectAttrubitesTry.GovernmentValidation
{
    public class GovernmentExistingAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dBContext = validationContext.GetService<ApplicationDbContext>();
            Government government = null;
            string name = (string)value;
            government = dBContext.Governments.FirstOrDefault(e=>e.Name.ToLower().Contains(name.ToLower()));

            if (government == null)
                return new ValidationResult(ErrorMessage ?? "Government already exist.");
            return ValidationResult.Success;
        }

    }
}
