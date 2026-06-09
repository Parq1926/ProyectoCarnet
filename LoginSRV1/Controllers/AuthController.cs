using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LoginSRV1.Data;
using LoginSRV1.Models;

namespace LoginSRV1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // ============================================================
    // POST: api/auth/login
    // ============================================================
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromHeader] string usuario,
        [FromHeader] string contrasena,
        [FromHeader] string tipo)
    {
        // Validar que los datos no sean nulos o vacíos
        if (string.IsNullOrWhiteSpace(usuario) ||
            string.IsNullOrWhiteSpace(contrasena) ||
            string.IsNullOrWhiteSpace(tipo))
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "Usuario y/o contraseña incorrectos"
            });
        }

        // Buscar usuario por email
        var user = await _context.Usuarios
            .Include(u => u.TipoUsuario)
            .FirstOrDefaultAsync(u => u.Email == usuario && u.Activo == true);

        // Validar credenciales
        if (user == null || user.Contrasena != contrasena)
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "Usuario y/o contraseña incorrectos"
            });
        }

        // Validar que el tipo de usuario coincida
        if (user.TipoUsuario?.Nombre != tipo)
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "Usuario y/o contraseña incorrectos"
            });
        }

        // Obtener tiempo de expiración de parámetros
        var jwtExpMin = await _context.Parametros
            .Where(p => p.Id == "JWT_EXP_MIN")
            .Select(p => int.Parse(p.Valor))
            .FirstOrDefaultAsync();

        if (jwtExpMin == 0) jwtExpMin = 5;

        var refreshExpDays = await _context.Parametros
            .Where(p => p.Id == "REFRESH_EXP_DAYS")
            .Select(p => int.Parse(p.Valor))
            .FirstOrDefaultAsync();

        if (refreshExpDays == 0) refreshExpDays = 7;

        // Generar JWT
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        var expiresIn = DateTime.Now.AddMinutes(jwtExpMin);

        // Guardar refresh token
        var session = new Sesion
        {
            UsuarioId = user.Id,
            RefreshToken = refreshToken,
            FechaExpiracion = DateTime.Now.AddDays(refreshExpDays),
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        _context.Sesiones.Add(session);
        await _context.SaveChangesAsync();

        // Respuesta exitosa 201 Created
        return StatusCode(201, new
        {
            codigo = 201,
            mensaje = "Login exitoso",
            expires_in = expiresIn,
            access_token = accessToken,
            refresh_token = refreshToken,
            usuarioID = user.Id,
            institutions = new[] { "CUC" }
        });
    }

    // ============================================================
    // POST: api/auth/refresh
    // ============================================================
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "No autorizado"
            });
        }

        var session = await _context.Sesiones
            .Include(s => s.Usuario)
            .ThenInclude(u => u!.TipoUsuario)
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken && s.Activo == true);

        if (session == null || session.FechaExpiracion < DateTime.Now)
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "No autorizado"
            });
        }

        var jwtExpMin = await _context.Parametros
            .Where(p => p.Id == "JWT_EXP_MIN")
            .Select(p => int.Parse(p.Valor))
            .FirstOrDefaultAsync();

        if (jwtExpMin == 0) jwtExpMin = 5;

        var refreshExpDays = await _context.Parametros
            .Where(p => p.Id == "REFRESH_EXP_DAYS")
            .Select(p => int.Parse(p.Valor))
            .FirstOrDefaultAsync();

        if (refreshExpDays == 0) refreshExpDays = 7;

        var newAccessToken = GenerateJwtToken(session.Usuario!);
        var newRefreshToken = GenerateRefreshToken();

        // Invalidar refresh token anterior
        session.Activo = false;
        _context.Sesiones.Update(session);

        // Guardar nuevo refresh token
        var newSession = new Sesion
        {
            UsuarioId = session.UsuarioId,
            RefreshToken = newRefreshToken,
            FechaExpiracion = DateTime.Now.AddDays(refreshExpDays),
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        _context.Sesiones.Add(newSession);
        await _context.SaveChangesAsync();

        return StatusCode(201, new
        {
            codigo = 201,
            mensaje = "Token refrescado exitosamente",
            expires_in = DateTime.Now.AddMinutes(jwtExpMin),
            access_token = newAccessToken,
            refresh_token = newRefreshToken
        });
    }

    // ============================================================
    // GET: api/auth/validate
    // ============================================================
    [HttpGet("validate")]
    public IActionResult Validate([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "Token no proporcionado"
            });
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Ok(new
            {
                codigo = 200,
                mensaje = "Token válido",
                valido = true
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "Token expirado",
                valido = false
            });
        }
        catch
        {
            return Unauthorized(new
            {
                codigo = 401,
                mensaje = "Token inválido",
                valido = false
            });
        }
    }

    // ============================================================
    // Métodos privados
    // ============================================================

    private string GenerateJwtToken(Usuario user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");

        var jwtExpMin = _context.Parametros
            .Where(p => p.Id == "JWT_EXP_MIN")
            .Select(p => int.Parse(p.Valor))
            .FirstOrDefault();

        if (jwtExpMin == 0) jwtExpMin = 5;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.NombreCompleto),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.TipoUsuario?.Nombre ?? "Usuario")
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(jwtExpMin),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

// Modelo para la solicitud de refresh
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}