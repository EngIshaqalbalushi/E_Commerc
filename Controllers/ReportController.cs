using E_CommerceSystem.DTOs.Reports;
using E_CommerceSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceSystem.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("best-selling-products")]
        public IActionResult BestSellingProducts(int top = 10)
        {
            return Ok(_reportService.GetBestSellingProducts(top));
        }

        [HttpGet("daily-revenue")]
        public IActionResult DailyRevenue()
        {
            return Ok(_reportService.GetDailyRevenue());
        }

        [HttpGet("monthly-revenue")]
        public IActionResult MonthlyRevenue()
        {
            return Ok(_reportService.GetMonthlyRevenue());
        }

        [HttpGet("top-rated-products")]
        public IActionResult TopRatedProducts(int top = 10)
        {
            return Ok(_reportService.GetTopRatedProducts(top));
        }

        [HttpGet("most-active-customers")]
        public IActionResult MostActiveCustomers(int top = 10)
        {
            return Ok(_reportService.GetMostActiveCustomers(top));
        }
    }
}
