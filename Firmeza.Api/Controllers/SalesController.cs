using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Api.Services;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly IGenericRepository<Sale> _saleRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<Client> _clientRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<SalesController> _logger;

    public SalesController(
        IGenericRepository<Sale> saleRepository,
        IGenericRepository<Product> productRepository,
        IGenericRepository<Client> clientRepository,
        IEmailService emailService,
        IMapper mapper,
        ILogger<SalesController> logger)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _clientRepository = clientRepository;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todas las ventas
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetAll()
    {
        try
        {
            var sales = await _saleRepository.GetAllAsync(
                include: query => query
                    .Include(s => s.Client)
                    .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
            );

            var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(sales);
            return Ok(saleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales");
            return StatusCode(500, new { message = "Error al obtener ventas" });
        }
    }

    /// <summary>
    /// Obtener una venta por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDto>> GetById(int id)
    {
        try
        {
            var sales = await _saleRepository.GetAllAsync(
                include: query => query
                    .Include(s => s.Client)
                    .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
            );

            var sale = sales.FirstOrDefault(s => s.id == id);
            if (sale == null)
                return NotFound(new { message = "Venta no encontrada" });

            var saleDto = _mapper.Map<SaleDto>(sale);
            return Ok(saleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sale {Id}", id);
            return StatusCode(500, new { message = "Error al obtener venta" });
        }
    }

    /// <summary>
    /// Crear una nueva venta
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SaleDto>> Create([FromBody] CreateSaleDto createSaleDto)
    {
        try
        {
            // Validar que el cliente existe
            var client = await _clientRepository.GetByIdAsync(createSaleDto.ClientId);
            if (client == null)
                return BadRequest(new { message = "Cliente no encontrado" });

            // Crear la venta
            var sale = new Sale
            {
                ClientId = createSaleDto.ClientId,
                Date = DateTime.UtcNow,
                SaleDetails = new List<SaleDetail>()
            };

            decimal total = 0;

            // Procesar cada detalle de venta
            foreach (var detailDto in createSaleDto.SaleDetails)
            {
                var product = await _productRepository.GetByIdAsync(detailDto.ProductId);
                if (product == null)
                    return BadRequest(new { message = $"Producto {detailDto.ProductId} no encontrado" });

                if (product.Stock < detailDto.Quantity)
                    return BadRequest(new { message = $"Stock insuficiente para el producto {product.Name}" });

                // Reducir stock
                product.RemoveStock(detailDto.Quantity);
                await _productRepository.UpdateAsync(product);

                // Crear detalle de venta
                var saleDetail = new SaleDetail
                {
                    ProductId = detailDto.ProductId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = product.Price
                };

                sale.SaleDetails.Add(saleDetail);
                total += saleDetail.Quantity * saleDetail.UnitPrice;
            }

            sale.Total = total;
            await _saleRepository.AddAsync(sale);

            // Enviar email de confirmación (sin esperar)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendPurchaseConfirmationAsync(
                        client.Email,
                        client.Name,
                        sale.id,
                        sale.Total
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending purchase confirmation email");
                }
            });

            // Cargar la venta completa para retornar
            var sales = await _saleRepository.GetAllAsync(
                include: query => query
                    .Include(s => s.Client)
                    .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
            );

            var createdSale = sales.FirstOrDefault(s => s.id == sale.id);
            var saleDto = _mapper.Map<SaleDto>(createdSale);

            return CreatedAtAction(nameof(GetById), new { id = sale.id }, saleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale");
            return StatusCode(500, new { message = "Error al crear venta: " + ex.Message });
        }
    }

    /// <summary>
    /// Obtener ventas por cliente
    /// </summary>
    [HttpGet("by-client/{clientId}")]
    public async Task<ActionResult<IEnumerable<SaleDto>>> GetByClient(int clientId)
    {
        try
        {
            var sales = await _saleRepository.GetAllAsync(
                include: query => query
                    .Include(s => s.Client)
                    .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
            );

            var clientSales = sales.Where(s => s.ClientId == clientId);
            var saleDtos = _mapper.Map<IEnumerable<SaleDto>>(clientSales);

            return Ok(saleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sales for client {ClientId}", clientId);
            return StatusCode(500, new { message = "Error al obtener ventas del cliente" });
        }
    }
}
