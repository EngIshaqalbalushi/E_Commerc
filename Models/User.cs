using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_CommerceSystem.Models
{
    public class User
    {
        [Key]
        public int UID { get; set; }

        [Required]
        public string UName { get; set; } = string.Empty;   // ✅ default init

        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            ErrorMessage = "Invalid email format (e.g. 'example@gmail.com')")]
        public string Email { get; set; } = string.Empty;   // ✅ default init

        [JsonIgnore]
        [Required]
        public string PasswordHash { get; set; } = string.Empty;   // ✅ default init

        [Required]
        public string Phone { get; set; } = string.Empty;   // ✅ default init

        [Required]
        public string Role { get; set; } = "Customer";   // ✅ default role

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔑 refresh token
        [JsonIgnore]
        public string? RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime? RefreshTokenExpiryTime { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();   // ✅ init

        [JsonIgnore]
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>(); // ✅ init
    }
}
