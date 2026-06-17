using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SRV1_Login.Data;
using SRV1_Login.Entities;
using SRV1_Login.Services;
using System.Text;

namespace SRV1_Login.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Auth").WithTags("Auth");

  
        group.MapPost("/login", async (
            [FromHeader] string usuario,
            [FromHeader] string contrasena,
            [FromHeader] string tipo,
            ApplicationDbContext db,
            IServiceProvider services) =>
        {
            // Validar que los datos no sean nulos o vacíos
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena) || string.IsNullOrWhiteSpace(tipo))
                return Results.BadRequest(new { codigo = 400, mensaje = "Usuario, contraseña y tipo son requeridos" });

            // Buscar usuario por email
            var user = await db.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Email == usuario && u.Activo == true);

            // Validar credenciales
            if (user == null || user.Contrasena != contrasena)
                return Results.Json(new { codigo = 401, mensaje = "Usuario y/o contraseña incorrectos" }, statusCode: 401);

            // Validar tipo de usuario
            if (user.TipoUsuario?.Nombre != tipo)
                return Results.Json(new { codigo = 401, mensaje = "Tipo de usuario no válido" }, statusCode: 401);

            var tokenService = services.GetRequiredService<ITokenService>();
            var accessToken = tokenService.GenerateJwtToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            // Obtener tiempos de expiración
            var jwtExpMin = await db.Parametros
                .Where(p => p.Id == "JWT_EXP_MIN")
                .Select(p => int.Parse(p.Valor))
                .FirstOrDefaultAsync();
            if (jwtExpMin == 0) jwtExpMin = 5;

            var refreshExpDays = await db.Parametros
                .Where(p => p.Id == "REFRESH_EXP_DAYS")
                .Select(p => int.Parse(p.Valor))
                .FirstOrDefaultAsync();
            if (refreshExpDays == 0) refreshExpDays = 7;

            // Guardar refresh token en la base de datos
            var session = new Sesion
            {
                UsuarioId = user.Id,
                RefreshToken = refreshToken,
                FechaExpiracion = DateTime.Now.AddDays(refreshExpDays),
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            db.Sesiones.Add(session);
            await db.SaveChangesAsync();

            // Respuesta exitosa
            return Results.Ok(new
            {
                codigo = 200,
                mensaje = "Login exitoso",
                expires_in = DateTime.Now.AddMinutes(jwtExpMin),
                access_token = accessToken,
                refresh_token = refreshToken,
                usuarioID = user.Id,
                institutions = new[] { "CUC" }
            });
        });

        //Refresh 
        group.MapPost("/refresh", async (
            [FromBody] RefreshTokenRequest request,
            ApplicationDbContext db,
            IServiceProvider services) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Results.Json(new { codigo = 401, mensaje = "No autorizado" }, statusCode: 401);

            // Buscar el refresh token en la BD
            var session = await db.Sesiones
                .Include(s => s.Usuario)
                .ThenInclude(u => u!.TipoUsuario)
                .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken && s.Activo == true);

            if (session == null || session.FechaExpiracion < DateTime.Now)
                return Results.Json(new { codigo = 401, mensaje = "No autorizado" }, statusCode: 401);

            // Obtener tiempos de expiración
            var jwtExpMin = await db.Parametros
                .Where(p => p.Id == "JWT_EXP_MIN")
                .Select(p => int.Parse(p.Valor))
                .FirstOrDefaultAsync();
            if (jwtExpMin == 0) jwtExpMin = 5;

            var refreshExpDays = await db.Parametros
                .Where(p => p.Id == "REFRESH_EXP_DAYS")
                .Select(p => int.Parse(p.Valor))
                .FirstOrDefaultAsync();
            if (refreshExpDays == 0) refreshExpDays = 7;

            var tokenService = services.GetRequiredService<ITokenService>();
            var newAccessToken = tokenService.GenerateJwtToken(session.Usuario!);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            // Invalidar refresh token anterior
            session.Activo = false;
            db.Sesiones.Update(session);

            // Guardar nuevo refresh token
            var newSession = new Sesion
            {
                UsuarioId = session.UsuarioId,
                RefreshToken = newRefreshToken,
                FechaExpiracion = DateTime.Now.AddDays(refreshExpDays),
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            db.Sesiones.Add(newSession);
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                codigo = 200,
                mensaje = "Token refrescado exitosamente",
                expires_in = DateTime.Now.AddMinutes(jwtExpMin),
                access_token = newAccessToken,
                refresh_token = newRefreshToken
            });
        });
        ///Validate
        group.MapGet("/validate", async (string token, IConfiguration config) =>
        {
            if (string.IsNullOrWhiteSpace(token))
                return Results.Unauthorized();

            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return Results.Ok(new
                {
                    codigo = 200,
                    mensaje = "Token válido",
                    valido = true
                });
            }
            catch (SecurityTokenExpiredException)
            {
                return Results.Json(new
                {
                    codigo = 401,
                    mensaje = "Token expirado",
                    valido = false
                }, statusCode: 401);
            }
            catch
            {
                return Results.Json(new
                {
                    codigo = 401,
                    mensaje = "Token inválido",
                    valido = false
                }, statusCode: 401);
            }
        });
    }
}

// Modelo para la solicitud de refresh
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}