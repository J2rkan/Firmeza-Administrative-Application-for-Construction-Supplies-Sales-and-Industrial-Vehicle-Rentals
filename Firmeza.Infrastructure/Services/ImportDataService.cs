using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Firmeza.Application.Interfaces;
using System.Linq.Expressions;
using Firmeza.Application.ViewModels;

namespace Firmeza.Infrastructure.Services
{
    public class ImportDataService : IImportDataService
    {
        private readonly IExcelParserService _parser;
        // Inyectamos los repositorios genéricos para guardar los datos normalizados
        private readonly IGenericRepository<Client> _clientRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Sale> _saleRepo;
        private readonly IGenericRepository<SaleDetail> _saleDetailRepo;


        public ImportDataService(
            IExcelParserService parser,
            IGenericRepository<Client> clientRepo,
            IGenericRepository<Product> productRepo,
            IGenericRepository<Sale> saleRepo,
            IGenericRepository<SaleDetail> saleDetailRepo)
        {
            _parser = parser;
            _clientRepo = clientRepo;
            _productRepo = productRepo;
            _saleRepo = saleRepo;
            _saleDetailRepo = saleDetailRepo;
        }

        public async Task<ImportLogViewModel> ImportDataAsync(Stream fileStream)
        {
            var log = new ImportLogViewModel();
            // 1. Obtener datos crudos del Excel
            var rawData = await _parser.ParseExcelDataAsync(fileStream);
            log.RecordsProcessed = rawData.Count;

            // Usaremos un diccionario como caché local para evitar consultas repetidas a la BD para el mismo cliente en este archivo.
            var clientCache = new Dictionary<string, Client>(StringComparer.OrdinalIgnoreCase);
            var productCache = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
            var importTimestamp = DateTime.UtcNow; // Usar una única marca de tiempo para toda la importación.
            
            // 2. Lógica de Normalización, Validación e Inserción
            foreach (var row in rawData)
            {
                // VALIDACIÓN BÁSICA DE CAMPOS
                // Asumimos que las columnas necesarias existen, pero validamos su contenido.
                if (!row.ContainsKey("ClientName") || string.IsNullOrWhiteSpace(row["ClientName"]))
                {
                    log.Errors.Add("Fila omitida: Nombre del cliente es obligatorio.");
                    continue;
                }
                
                // --- PROCESO DE NORMALIZACIÓN ---

                // NORMALIZACIÓN 1: CLIENTES
                var clientName = row["ClientName"];
                Client client;

                // Buscar en la caché primero
                if (!clientCache.TryGetValue(clientName, out client))
                {
                    // Si no está en caché, buscar en la base de datos
                    Expression<Func<Client, bool>> clientPredicate = c => c.Name.Equals(clientName);
                    client = (await _clientRepo.ListAsync(clientPredicate)).FirstOrDefault();

                    if (client == null)
                    {
                        // Si no existe en la BD, crearlo
                        client = new Client { Name = clientName, Document = row.GetValueOrDefault("ClientDocument", "N/A"), Email = row.GetValueOrDefault("ClientEmail", "N/A") };
                        await _clientRepo.AddAsync(client);
                        log.NewClientsCreated++;
                    }
                    // Añadir el cliente (existente o nuevo) a la caché para las siguientes filas.
                    clientCache[clientName] = client;
                }

                // NORMALIZACIÓN 2: PRODUCTOS (y crear una nueva venta)
                if (row.ContainsKey("ProductName") && decimal.TryParse(row.GetValueOrDefault("Price", "0"), out decimal price) && int.TryParse(row.GetValueOrDefault("Quantity", "0"), out int quantity))
                {
                    var productName = row["ProductName"];
                    Product product;

                    // Buscar en la caché de productos primero
                    if (!productCache.TryGetValue(productName, out product))
                    {
                        // Si no está en caché, buscar en la base de datos
                        Expression<Func<Product, bool>> productPredicate = p => p.Name.Equals(productName);
                        product = (await _productRepo.ListAsync(productPredicate)).FirstOrDefault();
                        
                        // Añadir a la caché incluso si es nulo, para no volver a buscarlo en la BD.
                        productCache[productName] = product;
                    }
                    
                    if (product == null)
                    {
                        log.Errors.Add($"Producto '{productName}' no encontrado. Se omitió la venta de esta fila.");
                        continue;
                    }
                    
                    // Crear la Venta (Asumimos una venta por fila para simplificar)
                    var sale = new Sale
                    {
                        Client = client,
                        Date = importTimestamp,
                        Total = price * quantity, 
                    };
                    await _saleRepo.AddAsync(sale);

                    // Crear el Detalle de Venta
                    var saleDetail = new SaleDetail
                    {
                        Sale = sale,
                        Product = product,
                        Quantity = quantity,
                        UnitPrice = price
                    };
                    await _saleDetailRepo.AddAsync(saleDetail);
                    log.SalesImported++;
                }
            }

            // 3. Guardar todos los cambios de todos los repositorios en una única transacción.
            // Esto asume que todos los repositorios comparten el mismo DbContext (Unit of Work).
            await _clientRepo.SaveChangesAsync(); 
            log.Success = log.Errors.Count == 0;

            return log;
        }
    }
}