namespace E_CommerceSystem.Models
{
    public class OrderSummaryDTO
    {
        public int OrderId { get; set; }
        public string? UserName { get; set; }   // allow null in case User is not loaded
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }      // ✅ Added: match Order.Status
        public List<OrderProductDTO> Products { get; set; } = new List<OrderProductDTO>();
    }

    public class OrderProductDTO
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
