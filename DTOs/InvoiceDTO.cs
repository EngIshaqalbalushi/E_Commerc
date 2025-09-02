namespace E_CommerceSystem.DTOs
{
    public class InvoiceDTO
    {
        // Header
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public string CustomerName { get; set; } = "";
        public string CustomerEmail { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string BillingAddress { get; set; } = "";

        // Lines
        public List<InvoiceLineDTO> Lines { get; set; } = new();
        public decimal Subtotal => Lines.Sum(l => l.LineTotal);
        public decimal Shipping { get; set; } = 0m;
        public decimal Discount { get; set; } = 0m;
        public decimal Tax { get; set; } = 0m; // add your tax calc if needed
        public decimal Total => Subtotal + Shipping + Tax - Discount;
    }

    public class InvoiceLineDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
