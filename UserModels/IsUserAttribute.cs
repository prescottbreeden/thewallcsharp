using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace login_reg.Models
{
    public class IsUserAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var LoginUser = (LoginUserModel)validationContext.ObjectInstance;
            var service = (DbConnector)validationContext.GetService(typeof(DbConnector));
            var emailCheck = service.Query($"SELECT id FROM users WHERE email = '{LoginUser.Email}'");
            if (emailCheck.Count == 0)
            {
                
                return new ValidationResult("Email not found.");
            }

            return ValidationResult.Success;
        }

    }
}