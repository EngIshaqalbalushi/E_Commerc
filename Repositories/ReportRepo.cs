using E_CommerceSystem.Models;
using E_CommerceSystem.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceSystem.Repositories
{
    public class ReportRepo
    {
        private readonly ApplicationDbContext _context;
        public ReportRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Best-selling products
        public IEnumerable<BestSellingProductDTO> GetBestSellingProducts(int top = 10)
        {
            return _context.OrderProducts
                .Include(op => op.Product)
                .GroupBy(op => new { op.PID, op.Product.ProductName })
                .Select(g => new BestSellingProductDTO
                {
                    ProductId = g.Key.PID,
                    ProductName = g.Key.ProductName,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(top)
                .ToList();
        }

        // ✅ Revenue report (per day)
        public IEnumerable<RevenueReportDTO> GetDailyRevenue()
        {
            return _context.Orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new RevenueReportDTO
                {
                    Period = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(r => r.Period)
                .ToList();
        }

        // ✅ Revenue report (per month)
        public IEnumerable<RevenueReportDTO> GetMonthlyRevenue()
        {
            return _context.Orders
                .GroupBy(o => new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1))
                .Select(g => new RevenueReportDTO
                {
                    Period = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(r => r.Period)
                .ToList();
        }

        // ✅ Top-rated products
        public IEnumerable<TopRatedProductDTO> GetTopRatedProducts(int top = 10)
        {
            return _context.Reviews
     .Include(r => r.Product)
     .GroupBy(r => new { r.PID, r.Product.ProductName })
     .Select(g => new TopRatedProductDTO
     {
         ProductId = g.Key.PID,
         ProductName = g.Key.ProductName,
         AverageRating = (decimal)g.Average(r => r.Rating)
     })
     .OrderByDescending(x => x.AverageRating)
     .Take(10)
     .ToList();

        }

        // ✅ Most active customers
        public IEnumerable<ActiveCustomerDTO> GetMostActiveCustomers(int top = 10)
        {
            return _context.Orders
                .Include(o => o.User)
                .GroupBy(o => new { o.UID, o.User.UName })
                .Select(g => new ActiveCustomerDTO
                {
                    UserId = g.Key.UID,
                    UserName = g.Key.UName,
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.OrdersCount)
                .Take(top)
                .ToList();
        }
    }
}
