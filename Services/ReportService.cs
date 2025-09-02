using E_CommerceSystem.DTOs.Reports;
using E_CommerceSystem.Repositories;
using E_CommerceSystem.Services;
namespace E_CommerceSystem.Services
{
    public class ReportService
    {
        private readonly ReportRepo _reportRepo;

        public ReportService(ReportRepo reportRepo)
        {
            _reportRepo = reportRepo;
        }

        public IEnumerable<BestSellingProductDTO> GetBestSellingProducts(int top = 10) =>
            _reportRepo.GetBestSellingProducts(top);

        public IEnumerable<RevenueReportDTO> GetDailyRevenue() =>
            _reportRepo.GetDailyRevenue();

        public IEnumerable<RevenueReportDTO> GetMonthlyRevenue() =>
            _reportRepo.GetMonthlyRevenue();

        public IEnumerable<TopRatedProductDTO> GetTopRatedProducts(int top = 10) =>
            _reportRepo.GetTopRatedProducts(top);

        public IEnumerable<ActiveCustomerDTO> GetMostActiveCustomers(int top = 10) =>
            _reportRepo.GetMostActiveCustomers(top);
    }
}
