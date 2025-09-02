using System.ComponentModel.DataAnnotations;

namespace E_CommerceSystem.DTOs
{
    public class LoginDTO
    {
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "Invalid email format (e.g. example@gmail.com)")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }   // raw password input
    }
}
