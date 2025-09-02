using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_CommerceSystem.Models
{
    public class Supplier
    {
        [Key] //pk 
        public int SID { get; set; }   // Supplier ID (short style like UID, PID, OID)

        [Required]
        public string Name { get; set; } //

        [Required]
        [EmailAddress] //email must be valid ..
        public string ContactEmail { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; } //phone must be valid

        // Navigation: One Supplier → Many Products
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }//because its many to one relationship.. one supplier can supply many products
    }
}
