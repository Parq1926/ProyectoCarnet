// Endpoints/LoginEndpoints.cs
using LoginSRV1.Data;
using LoginSRV1.DTOs;
using LoginSRV1.Entities;
using LoginSRV1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginSRV1.Endpoints
{
    public static class LoginEndpoints
    {
        public static void MapLoginEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/Auth")
                .WithTags(nameof(LoginEndpoints));

            // Login
            group.MapPost("/login", async (
                [FromBody] LoginDto login,
                [FromServices] ApplicationDbContext db,
                [FromServices] ITokenService tokenService) =>
            {
                if (login == null || string.IsNullOrWhiteSpace(login.Usuario) ||
                    string.IsNullOrWhiteSpace(login.Contrasena) || string.IsNullOrWhiteSpace(login.Tipo))
                {
                    return Results.BadRequest(new { codigo = 400, mensaje = "Usuario, contraseña y tipo son requeridos" });
                }

                // Determinar el ID del tipo según el nombre recibido
                int tipoId = login.Tipo switch
                {
                    "Administrador" => 1,
                    "Funcionario" => 2,
                    "Estudiante" => 3,
                    _ => 0
                };

                if (tipoId == 0)
                {
                    return Results.BadRequest(new { codigo = 400, mensaje = "Tipo de usuario no válido" });
                }

                // Buscar usuario por email y tipo
                var user = await db.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == login.Usuario && u.TipoUsuarioId == tipoId);

                // VERIFICAR SI EL USUARIO ESTÁ BLOQUEADO (manejando NULL)
                if (user != null && user.Bloqueado == true)
                {
                    return Results.Json(new
                    {
                        codigo = 403,
                        mensaje = "Usuario bloqueado permanentemente. Contacte al administrador."
                    }, statusCode: 403);
                }

                // Validar credenciales
                if (user == null || user.Contrasena != login.Contrasena)
                {
                    // Incrementar intentos fallidos
                    if (user != null)
                    {
                        // Obtener contador de intentos de la base de datos
                        var attempts = await db.Parametros
                            .FirstOrDefaultAsync(p => p.Id == $"LoginAttempts_{user.Id}");

                        int currentAttempts = 1;
                        if (attempts != null && !string.IsNullOrEmpty(attempts.Valor))
                        {
                            if (int.TryParse(attempts.Valor, out int value))
                            {
                                currentAttempts = value + 1;
                            }
                            attempts.Valor = currentAttempts.ToString();
                        }
                        else
                        {
                            var newAttempt = new Parametro
                            {
                                Id = $"LoginAttempts_{user.Id}",
                                Valor = "1",
                                Descripcion = "Intentos de login fallidos"
                            };
                            db.Parametros.Add(newAttempt);
                        }

                        // SI LLEGA A 3 INTENTOS, BLOQUEAR PERMANENTEMENTE
                        if (currentAttempts >= 3)
                        {
                            user.Bloqueado = true;
                            await db.SaveChangesAsync();
                            return Results.Json(new
                            {
                                codigo = 403,
                                mensaje = "Ha excedido el número de intentos. Usuario bloqueado permanentemente."
                            }, statusCode: 403);
                        }

                        await db.SaveChangesAsync();
                    }

                    return Results.Json(new
                    {
                        codigo = 401,
                        mensaje = "Usuario y/o contraseña incorrectos"
                    }, statusCode: 401);
                }

                // SI EL LOGIN ES EXITOSO, RESETEAR INTENTOS
                if (user != null)
                {
                    var attempts = await db.Parametros
                        .FirstOrDefaultAsync(p => p.Id == $"LoginAttempts_{user.Id}");
                    if (attempts != null)
                    {
                        attempts.Valor = "0";
                        await db.SaveChangesAsync();
                    }
                }

                // Generar tokens usando TokenService
                var accessToken = tokenService.GenerateJwtToken(user!);
                var refreshToken = tokenService.GenerateRefreshToken();

                var session = new Sesion
                {
                    UsuarioId = user!.Id,
                    RefreshToken = refreshToken,
                    FechaExpiracion = DateTime.Now.AddDays(7),
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                db.Sesiones.Add(session);
                await db.SaveChangesAsync();

                return Results.Ok(new LoginResponseDto
                {
                    Codigo = 200,
                    Mensaje = "Login exitoso",
                    ExpiresIn = DateTime.Now.AddMinutes(5),
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    UsuarioId = user.Id,
                    Institutions = new[] { "CUC" }
                });
            })
            .WithName("Login");

            // Endpoint de prueba para formularios
            group.MapPost("/test-form", async ([FromForm] string usuario, [FromForm] string contrasena, [FromForm] string tipo) =>
            {
                return Results.Ok(new
                {
                    usuario = usuario,
                    contrasena = contrasena,
                    tipo = tipo,
                    mensaje = "Formulario recibido correctamente"
                });
            })
            .WithName("TestForm");

            // Refresh
            group.MapPost("/refresh", async (
                [FromBody] RefreshTokenRequestDto request,
                [FromServices] ApplicationDbContext db,
                [FromServices] ITokenService tokenService) =>
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Results.Json(new { codigo = 401, mensaje = "No autorizado" }, statusCode: 401);
                }

                var session = await db.Sesiones
                    .Include(s => s.Usuario)
                    .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken && s.Activo);

                if (session == null || session.FechaExpiracion < DateTime.Now)
                {
                    return Results.Json(new { codigo = 401, mensaje = "No autorizado" }, statusCode: 401);
                }

                var newAccessToken = tokenService.GenerateJwtToken(session.Usuario!);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                session.Activo = false;
                db.Sesiones.Update(session);

                var newSession = new Sesion
                {
                    UsuarioId = session.UsuarioId,
                    RefreshToken = newRefreshToken,
                    FechaExpiracion = DateTime.Now.AddDays(7),
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                db.Sesiones.Add(newSession);
                await db.SaveChangesAsync();

                return Results.Ok(new RefreshResponseDto
                {
                    Codigo = 200,
                    Mensaje = "Token refrescado exitosamente",
                    ExpiresIn = DateTime.Now.AddMinutes(5),
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            })
            .WithName("RefreshToken");

            // Validate
            group.MapGet("/validate", async (
                [FromHeader(Name = "Authorization")] string authorization,
                [FromServices] IConfiguration config) =>
            {
                string? token = null;
                if (!string.IsNullOrWhiteSpace(authorization) && authorization.StartsWith("Bearer "))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }

                if (string.IsNullOrWhiteSpace(token))
                {
                    return Results.Json(new { codigo = 401, mensaje = "Token no proporcionado" }, statusCode: 401);
                }

                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
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

                    return Results.Ok(new TokenValidationDto
                    {
                        Codigo = 200,
                        Mensaje = "Token válido",
                        Valido = true
                    });
                }
                catch (SecurityTokenExpiredException)
                {
                    return Results.Json(new TokenValidationDto
                    {
                        Codigo = 401,
                        Mensaje = "Token expirado",
                        Valido = false
                    }, statusCode: 401);
                }
                catch
                {
                    return Results.Json(new TokenValidationDto
                    {
                        Codigo = 401,
                        Mensaje = "Token inválido",
                        Valido = false
                    }, statusCode: 401);
                }
            })
            .WithName("ValidateToken");
        }
    }
}