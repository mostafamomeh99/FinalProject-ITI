using Azure.Core;
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
  public  class GovernmentImageExisitngAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dBContext = validationContext.GetService<ApplicationDbContext>();
            Government government = null;
            string imageurl = value as string;
            if(imageurl == null)
            {
                return ValidationResult.Success;
            }
            string validimag = imageurl.Split('/').Last();
            government = dBContext.Governments.FirstOrDefault(e => e.Image == validimag);

            if (government == null)
                return new ValidationResult(ErrorMessage ?? "Government image required.");
            return ValidationResult.Success;
        }
    }
}
