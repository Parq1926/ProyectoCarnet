<<<<<<< HEAD
﻿// Endpoints/LoginEndpoints.cs
using LoginSRV1.Data;
=======
﻿using LoginSRV1.Data;
>>>>>>> a7a79ac (Actualizacion del Login)
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
<<<<<<< HEAD
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
=======
                [FromServices] IUsuarioApiClient usuarioApiClient,
                [FromServices] ApplicationDbContext db,
                [FromServices] ITokenService tokenService) =>
            {
                try
                {
                    // Validar datos
                    if (login == null || string.IsNullOrWhiteSpace(login.Usuario) ||
                        string.IsNullOrWhiteSpace(login.Contrasena) || string.IsNullOrWhiteSpace(login.Tipo))
                    {
                        return Results.BadRequest(new { codigo = 400, mensaje = "Usuario, contraseña y tipo son requeridos" });
                    }

                    // Consultar usuario en UsuariosSRV4
                    var credencialesResponse = await usuarioApiClient.ValidarCredencialesAsync(
                        login.Usuario,
                        login.Contrasena,
                        login.Tipo
                    );

                    if (credencialesResponse == null)
                    {
                        return Results.Json(new
                        {
                            codigo = 401,
                            mensaje = "Usuario y/o contraseña incorrectos"
                        }, statusCode: 401);
                    }

                    if (credencialesResponse.Bloqueado)
                    {
                        return Results.Json(new
                        {
                            codigo = 403,
                            mensaje = "Usuario bloqueado permanentemente. Contacte al administrador."
                        }, statusCode: 403);
                    }

                    if (!credencialesResponse.Activo)
                    {
                        return Results.Json(new
                        {
                            codigo = 403,
                            mensaje = "Usuario inactivo. Contacte al administrador."
                        }, statusCode: 403);
                    }

                    // ✅ Convertir ValidarCredencialesResponse a UsuarioDto
                    var user = new UsuarioDto
                    {
                        Id = credencialesResponse.Id,
                        Email = credencialesResponse.Email,
                        NombreCompleto = credencialesResponse.NombreCompleto,
                        TipoUsuario = credencialesResponse.TipoUsuario,
                        Activo = credencialesResponse.Activo,
                        Bloqueado = credencialesResponse.Bloqueado
                    };

                    // Obtener parámetros de expiración
                    var jwtExpParam = await db.Parametros
                        .FirstOrDefaultAsync(p => p.Id == "JWT_EXP_MIN");
                    var jwtExpMin = jwtExpParam != null && int.TryParse(jwtExpParam.Valor, out int jwt) ? jwt : 5;

                    var refreshExpParam = await db.Parametros
                        .FirstOrDefaultAsync(p => p.Id == "REFRESH_EXP_DAYS");
                    var refreshExpDays = refreshExpParam != null && int.TryParse(refreshExpParam.Valor, out int refresh) ? refresh : 7;

                    // ✅ Generar tokens usando UsuarioDto
                    var accessToken = tokenService.GenerateJwtToken(user);
                    var refreshToken = tokenService.GenerateRefreshToken();

                    // Guardar refresh token en SESION
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

                    // ✅ Usar LoginResponseDto
                    return Results.Ok(new LoginResponseDto
                    {
                        Codigo = 200,
                        Mensaje = "Login exitoso",
                        ExpiresIn = DateTime.Now.AddMinutes(jwtExpMin),
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        UsuarioId = user.Id,
                        Institutions = new[] { "CUC" }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error en login: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                    return Results.Json(new
                    {
                        codigo = 500,
                        mensaje = $"Error interno: {ex.Message}"
                    }, statusCode: 500);
                }
            })
            .WithName("Login");

            // Refresh Token
            group.MapPost("/refresh", async (
                [FromBody] RefreshTokenRequestDto request,
                [FromServices] ApplicationDbContext db,
                [FromServices] ITokenService tokenService,
                [FromServices] IUsuarioApiClient usuarioApiClient) =>
>>>>>>> a7a79ac (Actualizacion del Login)
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Results.Json(new { codigo = 401, mensaje = "No autorizado" }, statusCode: 401);
                }

                var session = await db.Sesiones
<<<<<<< HEAD
                    .Include(s => s.Usuario)
=======
>>>>>>> a7a79ac (Actualizacion del Login)
                    .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken && s.Activo);

                if (session == null || session.FechaExpiracion < DateTime.Now)
                {
                    return Results.Json(new { codigo = 401, mensaje = "No autorizado" }, statusCode: 401);
                }

<<<<<<< HEAD
                var newAccessToken = tokenService.GenerateJwtToken(session.Usuario!);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                session.Activo = false;
                db.Sesiones.Update(session);

=======
                // Obtener usuario desde UsuariosSRV4 (devuelve UsuarioDto)
                var user = await usuarioApiClient.GetUsuarioByIdAsync(session.UsuarioId);
                if (user == null)
                {
                    return Results.Json(new { codigo = 401, mensaje = "Usuario no encontrado" }, statusCode: 401);
                }

                var newAccessToken = tokenService.GenerateJwtToken(user);
                var newRefreshToken = tokenService.GenerateRefreshToken();

                // Invalidar refresh token anterior
                session.Activo = false;
                db.Sesiones.Update(session);

                // Guardar nuevo refresh token
>>>>>>> a7a79ac (Actualizacion del Login)
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

<<<<<<< HEAD
            // Validate

            group.MapGet("/validate", async (
                [FromHeader(Name = "Authorization")] string? authorization,
                [FromServices] IConfiguration config) =>
            {

                Console.WriteLine("HEADER RECIBIDO:");
                Console.WriteLine(authorization);


=======
            // Validate Token
            group.MapGet("/validate", async (
                [FromHeader(Name = "Authorization")] string authorization,
                [FromServices] IConfiguration config) =>
            {
>>>>>>> a7a79ac (Actualizacion del Login)
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
<<<<<<< HEAD
                catch
                {
=======
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error validando token: {ex.Message}");
>>>>>>> a7a79ac (Actualizacion del Login)
                    return Results.Json(new TokenValidationDto
                    {
                        Codigo = 401,
                        Mensaje = "Token inválido",
                        Valido = false
                    }, statusCode: 401);
                }
            })
            .WithName("ValidateToken");
<<<<<<< HEAD

=======
>>>>>>> a7a79ac (Actualizacion del Login)
        }
    }
}