using System.ComponentModel.DataAnnotations;

namespace E_CommerceSystem.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string UName { get; set; }

        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "Invalid email format (e.g. example@gmail.com)")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must be at least 8 chars long, contain an uppercase, lowercase, digit, and special char.")]
        public string Password { get; set; }

        [Required]
        public string Phone { get; set; }

        // optional – default is Customer
        public string Role { get; set; } = "Customer";
    }
}
