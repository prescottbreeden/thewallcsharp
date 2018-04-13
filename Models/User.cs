using System;
using System.ComponentModel.DataAnnotations;

namespace login_reg.Models
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }
        [Required]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [UniqueEmail]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password and confirmation must match.")]
        public string cPassword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User()
        {
            
        }

    }
}