using Firmeza.Core.Entities;

namespace Firmeza.Application.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalClients { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalRevenueAllTime { get; set; }
        public int LowStockProducts { get; set; }
        public List<Sale> RecentSales { get; set; } = new List<Sale>();
    }
}