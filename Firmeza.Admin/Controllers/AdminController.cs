using Firmeza.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Firmeza.Infrastructure.Persistence;

namespace Firmeza.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalClients = await _context.Clients.CountAsync(),
                TotalSales = await _context.Sales.CountAsync(),
                TotalRevenueToday = await _context.Sales
                    .Where(s => s.Date.Date == DateTime.Today)
                    .SumAsync(s => s.Total)
            };
            return View(viewModel);
        }
    }
}