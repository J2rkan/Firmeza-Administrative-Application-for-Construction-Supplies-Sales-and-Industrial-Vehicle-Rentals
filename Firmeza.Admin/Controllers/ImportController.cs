using Firmeza.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ImportController : Controller
    {
        private readonly IImportDataService _importService;

        public ImportController(IImportDataService importService)
        {
            _importService = importService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError("", "Por favor, selecciona un archivo Excel válido.");
                return View("Index");
            }

            var fileExtension = Path.GetExtension(excelFile.FileName).ToLowerInvariant();
            if (fileExtension != ".xlsx" && fileExtension != ".xls")
            {
                ModelState.AddModelError("", "El archivo debe ser un documento Excel (.xlsx o .xls).");
                return View("Index");
            }

            if (excelFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("", "El archivo no debe exceder los 5 MB.");
                return View("Index");
            }

            try
            {
                using (var stream = excelFile.OpenReadStream())
                {
                    var log = await _importService.ImportDataAsync(stream);
                    return View("Result", log);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al procesar el archivo: {ex.Message}");
                return View("Index");
            }
        }
    }
}
