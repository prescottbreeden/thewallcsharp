using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace login_reg.Models
{
    public class LoginUserModel
    {
        [Required]
        [EmailAddress]
        [IsUser]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        public LoginUserModel()
        {
            
        }
    }
}