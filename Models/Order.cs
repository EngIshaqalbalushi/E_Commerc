using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_CommerceSystem.Models
{
    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        [Key]
        public int OID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }



        // ✅ Only keep this one
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Foreign Key
        public int UID { get; set; }

        [ForeignKey("UID")]
        [JsonIgnore]
        public virtual User User { get; set; }

        // Navigation to OrderProducts
        public virtual ICollection<OrderProducts> OrderProducts { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
