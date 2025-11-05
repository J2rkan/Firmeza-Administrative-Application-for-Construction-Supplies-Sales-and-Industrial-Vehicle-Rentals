using Firmeza.Core.Entities; // Para la entidad Product
using Firmeza.Core.Interfaces; // Para IGenericRepository
using Firmeza.Application.ViewModels; // Para los ViewModels de Producto
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Admin.Controllers
{
    // Solo administradores pueden usar este controlador
    [Authorize(Roles = "Administrator")]
    public class ProductController : Controller
    {
        // Inyectamos el repositorio genérico para la entidad Product
        private readonly IGenericRepository<Product> _productRepository;

        public ProductController(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        // ------------------------------------------------------------------
        // READ: Listar Productos (Index)
        // ------------------------------------------------------------------
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.ListAllAsync();
            // Pasamos la lista de entidades a la vista
            return View(products);
        }

        // ------------------------------------------------------------------
        // CREATE: Crear Producto - [GET] Muestra el formulario
        // ------------------------------------------------------------------
        public IActionResult Create()
        {
            return View();
        }

        // ------------------------------------------------------------------
        // CREATE: Crear Producto - [POST] Recibe y guarda los datos
        // ------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Mapeamos el ViewModel a la entidad (sin usar AutoMapper por ahora)
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Stock = model.Stock
                };

                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // ------------------------------------------------------------------
        // DELETE: Eliminar Producto - [GET] Muestra la confirmación
        // ------------------------------------------------------------------
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product); // Mostramos la entidad completa para confirmar
        }

        // ------------------------------------------------------------------
        // DELETE: Eliminar Producto - [POST] Confirma la eliminación
        // ------------------------------------------------------------------
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
                await _productRepository.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ------------------------------------------------------------------
        // EDIT: Editar Producto - [GET] Muestra el formulario con datos
        // ------------------------------------------------------------------
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Mapeamos la entidad al ViewModel para edición
            var model = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock
            };

            return View(model);
        }

        // ------------------------------------------------------------------
        // EDIT: Editar Producto - [POST] Recibe y guarda los cambios
        // ------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
        {
            // Verificamos que el ID de la ruta coincida con el ID del modelo
            if (id != model.Id || !ModelState.IsValid)
            {
                if (id != model.Id) return NotFound();
                return View(model);
            }

            // Mapeamos el ViewModel a la entidad, incluyendo el ID
            var product = new Product
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock
            };

            // La función UpdateAsync del repositorio genérico se encarga de
            // marcar esta entidad como modificada para EF Core.
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}