using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace login_reg.Models
{
    public class UserModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress, UniqueEmail]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [DisplayName("Confirm")]
        public string Cpassword { get; set; }
        public UserModel()
        {
            
        }
    }
}