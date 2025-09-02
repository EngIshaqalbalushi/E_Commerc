using System.ComponentModel.DataAnnotations;//because models are already using Data Annotations [required] [key] etc..
using System.Text.Json.Serialization;//because we used [JsonIgnore]

namespace E_CommerceSystem.Models
{
    public class Category
    {
        [Key]
        public int CID { get; set; }   // keep it short like UID, PID, OID

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation: One Category → Many Products
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}
