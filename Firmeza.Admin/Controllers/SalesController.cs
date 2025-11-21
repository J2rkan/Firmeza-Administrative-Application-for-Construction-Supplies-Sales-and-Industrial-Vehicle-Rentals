using Firmeza.Application.Interfaces;
using OfficeOpenXml;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SalesController : Controller
    {
        private readonly IGenericRepository<Sale> _saleRepository;
        private readonly IPdfService _pdfService;

        public SalesController(IGenericRepository<Sale> saleRepository, IPdfService pdfService)
        {
            _saleRepository = saleRepository;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _saleRepository.ListAllWithIncludesAsync(s => s.Client);
            var orderedSales = sales.OrderByDescending(s => s.Date).ToList();
            return View(orderedSales);
        }

        public async Task<IActionResult> DownloadReceipt(int id)
        {
            var sale = await _saleRepository.GetByIdWithIncludesAsync(id, 
                s => s.Client, 
                s => s.SaleDetails);

            if (sale == null)
            {
                return NotFound();
            }

            var pdfBytes = _pdfService.GenerateSaleReceipt(sale);
            return File(pdfBytes, "application/pdf", $"Recibo_Venta_{id}.pdf");
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var sales = await _saleRepository.ListAllWithIncludesAsync(s => s.Client);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Ventas");

                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Fecha";
                worksheet.Cells[1, 3].Value = "Cliente";
                worksheet.Cells[1, 4].Value = "Total";

                int row = 2;
                foreach (var s in sales)
                {
                    worksheet.Cells[row, 1].Value = s.id;
                    worksheet.Cells[row, 2].Value = s.Date.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cells[row, 3].Value = s.Client?.Name ?? "N/A";
                    worksheet.Cells[row, 4].Value = s.Total;
                    row++;
                }

                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Ventas_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}
