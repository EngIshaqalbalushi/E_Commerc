using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_CommerceSystem.Models
{
    [PrimaryKey(nameof(OID), nameof(PID))]
    public class OrderProducts
    {
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [ForeignKey("Order")]
        public int OID { get; set; }

        [JsonIgnore]
        public virtual Order Order { get; set; }   // ✅ keep this one only

        [ForeignKey("Product")]
        public int PID { get; set; }

        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
