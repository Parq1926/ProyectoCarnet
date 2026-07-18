// UsuariosSRV4/Endpoints/UsuarioEndpoints.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsuariosSRV4.DTOs;
using UsuariosSRV4.Services;

namespace UsuariosSRV4.Endpoints
{
    public static class UsuarioEndpoints
    {
        public static void MapUsuarioEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/Usuarios")
                .WithTags("Usuarios");

            // ✅ ENDPOINTS PÚBLICOS (sin autenticación)
            // POST: /api/Usuarios/validar-credenciales
            group.MapPost("/validar-credenciales", async (
                [FromBody] ValidarCredencialesRequest request,
                [FromServices] IUsuarioService service) =>
            {
                try
                {
                    var result = await service.ValidarCredencialesAsync(request.Email, request.Password, request.Tipo);
                    if (result == null)
                    {
                        return Results.NotFound(new { mensaje = "Usuario o contraseña incorrectos" });
                    }
                    return Results.Ok(result);
                }
                catch (Exception ex)
                {
                    return Results.Json(new { error = ex.Message }, statusCode: 500);
                }
            })
            .WithName("ValidarCredenciales")
            .AllowAnonymous(); // ← PERMITIR SIN AUTENTICACIÓN

            // ✅ ENDPOINTS PROTEGIDOS (requieren autenticación)
            // GET: /api/Usuarios - Obtener todos
            group.MapGet("/", async (IUsuarioService service) =>
            {
                var result = await service.GetAllAsync();
                return Results.Ok(result);
            })
            .WithName("GetAllUsuarios")
            .RequireAuthorization(); // ← Requiere autenticación

            // GET: /api/Usuarios/{id}
            group.MapGet("/{id}", async (int id, IUsuarioService service) =>
            {
                var result = await service.GetByIdAsync(id);
                if (result == null)
                {
                    return Results.NotFound(new { error = $"Usuario con ID {id} no encontrado" });
                }
                return Results.Ok(result);
            })
            .WithName("GetUsuarioById")
            .RequireAuthorization(); // ← Requiere autenticación

            // UsuariosSRV4/Endpoints/UsuarioEndpoints.cs
            group.MapPut("/{id}/desbloquear", async (int id, IUsuarioService service) =>
            {
                var result = await service.DesbloquearUsuarioAsync(id);
                if (!result)
                {
                    return Results.NotFound(new { error = "Usuario no encontrado" });
                }
                return Results.Ok(new { message = "Usuario desbloqueado exitosamente" });
            })
            .WithName("DesbloquearUsuario")
            .RequireAuthorization();

            // POST: /api/Usuarios/buscar - Buscar por filtros
            group.MapPost("/buscar", async ([FromBody] FiltroUsuarioDto filtro, IUsuarioService service) =>
            {
                var result = await service.GetByFilterAsync(filtro);
                return Results.Ok(result);
            })
            .WithName("GetUsuariosByFilter")
            .RequireAuthorization(); // ← Requiere autenticación

            // POST: /api/Usuarios - Crear usuario
            group.MapPost("/", async ([FromBody] CrearUsuarioDto dto, IUsuarioService service) =>
            {
                var (ok, error, data) = await service.CreateAsync(dto);
                if (!ok)
                {
                    return Results.Conflict(new { error = error });
                }
                return Results.Created($"/api/Usuarios/{data?.Id}", data);
            })
            .WithName("CreateUsuario")
            .RequireAuthorization(); // ← Requiere autenticación

            // PUT: /api/Usuarios/{id} - Actualizar usuario
            group.MapPut("/{id}", async (int id, [FromBody] ActualizarUsuarioDto dto, IUsuarioService service) =>
            {
                if (id != dto.Id)
                {
                    return Results.BadRequest(new { error = "El ID de la ruta no coincide con el del cuerpo" });
                }

                var (ok, error, data) = await service.UpdateAsync(id, dto);
                if (!ok)
                {
                    if (error.Contains("no encontrado"))
                        return Results.NotFound(new { error = error });
                    return Results.Conflict(new { error = error });
                }
                return Results.Ok(data);
            })
            .WithName("UpdateUsuario")
            .RequireAuthorization(); // ← Requiere autenticación

            // DELETE: /api/Usuarios/{id} - Eliminar usuario
            group.MapDelete("/{id}", async (int id, IUsuarioService service) =>
            {
                var (ok, error) = await service.DeleteAsync(id);
                if (!ok)
                {
                    return Results.NotFound(new { error = error });
                }
                return Results.Ok(new { message = "Usuario eliminado correctamente" });
            })
            .WithName("DeleteUsuario")
            .RequireAuthorization(); // ← Requiere autenticación
        }
    }
}