using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRV1_Login.Data;
using SRV1_Login.Services;

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

            // Respuesta exitosa 200 OK
            return Results.Ok(new
            {
                codigo = 200,
                mensaje = "Login exitoso",
                access_token = accessToken,
                refresh_token = refreshToken,
                usuarioID = user.Id,
                expires_in = DateTime.Now.AddMinutes(5),
                institutions = new[] { "CUC" }
            });
        });
    }
}