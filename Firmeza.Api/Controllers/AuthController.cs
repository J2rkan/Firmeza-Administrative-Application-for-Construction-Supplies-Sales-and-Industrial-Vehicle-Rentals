using Firmeza.Api.DTOs;
using Firmeza.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Firmeza.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IEmailService emailService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuario
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Registro de nuevo cliente
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto, "Client");
            if (result == null)
                return BadRequest(new { message = "Error al registrar usuario. Verifica que las contraseñas coincidan." });

            // Enviar email de bienvenida (sin esperar)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendWelcomeEmailAsync(registerDto.Email, registerDto.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending welcome email to {Email}", registerDto.Email);
                }
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Registro de nuevo administrador (solo para testing)
    /// </summary>
    [HttpPost("register-admin")]
    public async Task<ActionResult<AuthResponseDto>> RegisterAdmin([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto, "Administrator");
            if (result == null)
                return BadRequest(new { message = "Error al registrar administrador" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin registration");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }
}
