using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Firmeza.Admin.Models;
using Firmeza.Application.ViewModels;

namespace Firmeza.Admin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Dashboard Administrativo";
        
        var model = new DashboardViewModel 
        { 
            TotalProducts = 0,
            TotalClients = 0,
            TotalSales = 0,
            TotalRevenueToday = 0m,
            TotalRevenueThisMonth = 0m,
            TotalRevenueAllTime = 0m,
            LowStockProducts = 0,
            RecentSales = new List<Firmeza.Core.Entities.Sale>()
        };
        
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}