using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRV6_TipoIdentificacion.Data;
using SRV6_TipoIdentificacion.DTOs;
using SRV6_TipoIdentificacion.Entities;

namespace SRV6_TipoIdentificacion.Endpoints;

public static class TipoIdentificacionEndpoints
{
    public static void MapTipoIdentificacionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TipoIdentificacion").WithTags("TipoIdentificacion")
            .WithTags("TipoIdentificacion")
            .RequireAuthorization(); 

        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var tipos = await db.TiposIdentificacion
                .Select(t => new TipoIdentificacionDto { Id = t.Id, Nombre = t.Nombre })
                .ToListAsync();
            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipos });
        });

        group.MapGet("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var tipo = await db.TiposIdentificacion.FindAsync(id);
            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de identificación con ID {id} no encontrado" });
            return Results.Ok(new { codigo = 200, mensaje = "OK", data = new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre } });
        });

        group.MapPost("/", async ([FromBody] CrearTipoIdentificacionDto dto, ApplicationDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Results.BadRequest(new { codigo = 400, mensaje = "El nombre es requerido" });

            var existe = await db.TiposIdentificacion.AnyAsync(t => t.Nombre == dto.Nombre);
            if (existe)
                return Results.Conflict(new { codigo = 409, mensaje = $"El tipo de identificación '{dto.Nombre}' ya existe" });

            var tipo = new TipoIdentificacion { Nombre = dto.Nombre };
            db.TiposIdentificacion.Add(tipo);
            await db.SaveChangesAsync();

            return Results.Created($"/api/TipoIdentificacion/{tipo.Id}", new
            {
                codigo = 201,
                mensaje = "Tipo de identificación creado exitosamente",
                data = new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre }
            });
        });

        group.MapPut("/{id}", async (int id, [FromBody] CrearTipoIdentificacionDto dto, ApplicationDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Results.BadRequest(new { codigo = 400, mensaje = "El nombre es requerido" });

            var tipo = await db.TiposIdentificacion.FindAsync(id);
            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de identificación con ID {id} no encontrado" });

            var existe = await db.TiposIdentificacion.AnyAsync(t => t.Nombre == dto.Nombre && t.Id != id);
            if (existe)
                return Results.Conflict(new { codigo = 409, mensaje = $"Ya existe otro tipo con el nombre '{dto.Nombre}'" });

            var nombreAnterior = tipo.Nombre;
            tipo.Nombre = dto.Nombre;
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                codigo = 200,
                mensaje = $"Tipo de identificación actualizado exitosamente. Se cambió de '{nombreAnterior}' a '{tipo.Nombre}'",
                anterior = nombreAnterior,
                nuevo = tipo.Nombre,
                data = new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre }
            });
        });

        group.MapDelete("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var tipo = await db.TiposIdentificacion.FindAsync(id);
            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de identificación con ID {id} no encontrado" });

            var nombreEliminado = tipo.Nombre;
            db.TiposIdentificacion.Remove(tipo);
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                codigo = 200,
                mensaje = $"Tipo de identificación '{nombreEliminado}' (ID: {id}) eliminado exitosamente",
                eliminado = nombreEliminado,
                id = id
            });
        });
    }
}