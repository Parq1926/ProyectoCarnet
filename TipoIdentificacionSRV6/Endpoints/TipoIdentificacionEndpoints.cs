using Microsoft.AspNetCore.Mvc;
using SRV6_TipoIdentificacion.DTOs;
using SRV6_TipoIdentificacion.Interfaces;

namespace SRV6_TipoIdentificacion.Endpoints;

public static class TipoIdentificacionEndpoints
{
    public static void MapTipoIdentificacionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TipoIdentificacion")
            .WithTags("TipoIdentificacion")
            .RequireAuthorization();

        // GET /api/TipoIdentificacion - Listar todos
        group.MapGet("/", async (ITipoIdentificacionService service) =>
        {
            var tipos = await service.GetAllAsync();
            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipos });
        });

        // GET /api/TipoIdentificacion/{id} - Obtener por ID
        group.MapGet("/{id}", async (int id, ITipoIdentificacionService service) =>
        {
            var tipo = await service.GetByIdAsync(id);
            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de identificación con ID {id} no encontrado" });

            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipo });
        });

        // POST /api/TipoIdentificacion - Crear nuevo
        group.MapPost("/", async ([FromBody] CrearTipoIdentificacionDto dto, ITipoIdentificacionService service) =>
        {
            var (ok, error, data) = await service.CreateAsync(dto);

            if (!ok)
            {
                if (error.Contains("ya existe"))
                    return Results.Conflict(new { codigo = 409, mensaje = error });

                return Results.BadRequest(new { codigo = 400, mensaje = error });
            }

            return Results.Created($"/api/TipoIdentificacion/{data?.Id}", new
            {
                codigo = 201,
                mensaje = "Tipo de identificación creado exitosamente",
                data = data
            });
        });

        // PUT /api/TipoIdentificacion/{id} - Actualizar
        group.MapPut("/{id}", async (int id, [FromBody] CrearTipoIdentificacionDto dto, ITipoIdentificacionService service) =>
        {
            var (ok, error, data) = await service.UpdateAsync(id, dto);

            if (!ok)
            {
                if (error.Contains("encontrado"))
                    return Results.NotFound(new { codigo = 404, mensaje = error });

                if (error.Contains("ya existe"))
                    return Results.Conflict(new { codigo = 409, mensaje = error });

                return Results.BadRequest(new { codigo = 400, mensaje = error });
            }

            return Results.Ok(new
            {
                codigo = 200,
                mensaje = $"Tipo de identificación actualizado exitosamente. {error}",
                anterior = error.Split("'")[1] ?? "",
                nuevo = error.Split("'")[3] ?? "",
                data = data
            });
        });

        // DELETE /api/TipoIdentificacion/{id} - Eliminar
        group.MapDelete("/{id}", async (int id, ITipoIdentificacionService service) =>
        {
            var (ok, error) = await service.DeleteAsync(id);

            if (!ok)
                return Results.NotFound(new { codigo = 404, mensaje = error });

            return Results.Ok(new { codigo = 200, mensaje = error });
        });
    }
}