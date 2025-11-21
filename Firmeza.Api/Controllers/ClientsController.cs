using AutoMapper;
using Firmeza.Api.DTOs;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class ClientsController : ControllerBase
{
    private readonly IGenericRepository<Client> _clientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(
        IGenericRepository<Client> clientRepository,
        IMapper mapper,
        ILogger<ClientsController> logger)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtener todos los clientes
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll([FromQuery] string? search = null)
    {
        try
        {
            var clients = await _clientRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                clients = clients.Where(c =>
                    c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    c.Document.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var clientDtos = _mapper.Map<IEnumerable<ClientDto>>(clients);
            return Ok(clientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients");
            return StatusCode(500, new { message = "Error al obtener clientes" });
        }
    }

    /// <summary>
    /// Obtener un cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetById(int id)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                return NotFound(new { message = "Cliente no encontrado" });

            var clientDto = _mapper.Map<ClientDto>(client);
            return Ok(clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client {Id}", id);
            return StatusCode(500, new { message = "Error al obtener cliente" });
        }
    }

    /// <summary>
    /// Crear un nuevo cliente
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create([FromBody] CreateClientDto createClientDto)
    {
        try
        {
            var client = _mapper.Map<Client>(createClientDto);
            await _clientRepository.AddAsync(client);

            var clientDto = _mapper.Map<ClientDto>(client);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating client");
            return StatusCode(500, new { message = "Error al crear cliente" });
        }
    }

    /// <summary>
    /// Actualizar un cliente existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ClientDto>> Update(int id, [FromBody] UpdateClientDto updateClientDto)
    {
        try
        {
            var existingClient = await _clientRepository.GetByIdAsync(id);
            if (existingClient == null)
                return NotFound(new { message = "Cliente no encontrado" });

            _mapper.Map(updateClientDto, existingClient);
            await _clientRepository.UpdateAsync(existingClient);

            var clientDto = _mapper.Map<ClientDto>(existingClient);
            return Ok(clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating client {Id}", id);
            return StatusCode(500, new { message = "Error al actualizar cliente" });
        }
    }

    /// <summary>
    /// Eliminar un cliente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                return NotFound(new { message = "Cliente no encontrado" });

            await _clientRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting client {Id}", id);
            return StatusCode(500, new { message = "Error al eliminar cliente" });
        }
    }
}
