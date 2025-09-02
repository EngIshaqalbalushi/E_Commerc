namespace E_CommerceSystem.DTOs.Reports
{
    public class RevenueReportDTO
    {
        public DateTime Period { get; set; }   // Day or Month
        public decimal TotalRevenue { get; set; }
    }
}