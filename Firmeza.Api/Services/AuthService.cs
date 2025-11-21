using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Firmeza.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Firmeza.Api.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto, string role = "Client");
}

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles.ToList());

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Roles = roles.ToList(),
            Expiration = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto, string role = "Client")
    {
        if (registerDto.Password != registerDto.ConfirmPassword)
            return null;

        var user = new IdentityUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
            return null;

        await _userManager.AddToRoleAsync(user, role);

        var roles = new List<string> { role };
        var token = GenerateJwtToken(user, roles);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Roles = roles,
            Expiration = DateTime.UtcNow.AddHours(24)
        };
    }

    private string GenerateJwtToken(IdentityUser user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
