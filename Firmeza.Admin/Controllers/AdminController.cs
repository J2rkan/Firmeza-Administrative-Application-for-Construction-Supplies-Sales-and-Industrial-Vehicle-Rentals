using Microsoft.AspNetCore.Mvc;
using Firmeza.Application.ViewModels;
using Firmeza.Core.Entities;

namespace Firmeza.Admin.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel 
            { 
                TotalProducts = 0,
                TotalClients = 0,
                TotalSales = 0,
                TotalRevenueToday = 0m,
                TotalRevenueThisMonth = 0m,
                TotalRevenueAllTime = 0m,
                LowStockProducts = 0,
                RecentSales = new List<Sale>()
            };
            
            return View(model);
        }
    }
}