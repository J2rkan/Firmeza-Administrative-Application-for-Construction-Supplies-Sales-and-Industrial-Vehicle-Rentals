using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firmeza.Application.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalClients { get; set; }
        public int TotalSales { get; set; }

        public decimal TotalRevenueToday { get; set; }
    }
}
