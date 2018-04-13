using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace login_reg.Models
{
    public class UniqueEmailAttribute : ValidationAttribute, IClientModelValidator
    {

        public void AddValidation(ClientModelValidationContext context)
        {
            // throw new NotImplementedException();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var newUser = (User)validationContext.ObjectInstance;

            var service = (DbConnector)validationContext.GetService(typeof(DbConnector));
            
            var emailCheck = service.Query($"SELECT id FROM users WHERE email = '{newUser.Email}'");
            
            if (emailCheck.Count != 0)
            {
                
                return new ValidationResult("email already registered");
            }

            return ValidationResult.Success;
        }

    }
}