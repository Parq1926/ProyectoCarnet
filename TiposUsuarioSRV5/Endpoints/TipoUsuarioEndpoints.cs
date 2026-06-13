using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SRV5_TipoUsuario.Data;
using SRV5_TipoUsuario.DTOs;
using SRV5_TipoUsuario.Entities;

namespace SRV5_TipoUsuario.Endpoints;

public static class TipoUsuarioEndpoints
{
    public static void MapTipoUsuarioEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TipoUsuario").WithTags("TipoUsuario")
            .WithTags("TipoUsuario")
            .RequireAuthorization();

        // GET /api/TipoUsuario - Listar todos
        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var tipos = await db.TiposUsuario
                .Select(t => new TipoUsuarioDto { Id = t.Id, Nombre = t.Nombre })
                .ToListAsync();

            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipos });
        });

        // GET /api/TipoUsuario/{id} - Obtener por ID
        group.MapGet("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var tipo = await db.TiposUsuario.FindAsync(id);
            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de usuario con ID {id} no encontrado" });

            return Results.Ok(new { codigo = 200, mensaje = "OK", data = new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre } });
        });

        // POST /api/TipoUsuario - Crear nuevo
        group.MapPost("/", async ([FromBody] CrearTipoUsuarioDto dto, ApplicationDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Results.BadRequest(new { codigo = 400, mensaje = "El nombre es requerido" });

            var existe = await db.TiposUsuario.AnyAsync(t => t.Nombre == dto.Nombre);
            if (existe)
                return Results.Conflict(new { codigo = 409, mensaje = $"El tipo de usuario '{dto.Nombre}' ya existe" });

            var tipo = new TipoUsuario { Nombre = dto.Nombre };
            db.TiposUsuario.Add(tipo);
            await db.SaveChangesAsync();

            return Results.Created($"/api/TipoUsuario/{tipo.Id}", new
            {
                codigo = 201,
                mensaje = $"Tipo de usuario '{tipo.Nombre}' creado exitosamente con ID {tipo.Id}",
                data = new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre }
            });
        });

        // PUT /api/TipoUsuario/{id} - Actualizar
        group.MapPut("/{id}", async (int id, [FromBody] CrearTipoUsuarioDto dto, ApplicationDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return Results.BadRequest(new { codigo = 400, mensaje = "El nombre es requerido" });

            var tipo = await db.TiposUsuario.FindAsync(id);
            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de usuario con ID {id} no encontrado" });

            var existe = await db.TiposUsuario.AnyAsync(t => t.Nombre == dto.Nombre && t.Id != id);
            if (existe)
                return Results.Conflict(new { codigo = 409, mensaje = $"Ya existe otro tipo con el nombre '{dto.Nombre}'" });

            var nombreAnterior = tipo.Nombre;  // ← Guardar el valor anterior
            tipo.Nombre = dto.Nombre;
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                codigo = 200,
                mensaje = $"Tipo de usuario actualizado exitosamente. Se cambió de '{nombreAnterior}' a '{tipo.Nombre}'",
                anterior = nombreAnterior,
                nuevo = tipo.Nombre,
                data = new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre }
            });
        });

        // DELETE /api/TipoUsuario/{id} - Eliminar
        group.MapDelete("/{id}", async (int id, ApplicationDbContext db) =>
        {
            var tipo = await db.TiposUsuario.FindAsync(id);
            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de usuario con ID {id} no encontrado" });

            var nombreEliminado = tipo.Nombre;  // ← Guardar el valor antes de eliminar
            db.TiposUsuario.Remove(tipo);
            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                codigo = 200,
                mensaje = $"Tipo de usuario '{nombreEliminado}' (ID: {id}) eliminado exitosamente",
                eliminado = nombreEliminado,
                id = id
            });
        });

        // GET /api/TipoUsuario/validar/{nombre} - Validar existencia
        group.MapGet("/validar/{nombre}", async (string nombre, ApplicationDbContext db) =>
        {
            var existe = await db.TiposUsuario.AnyAsync(t => t.Nombre == nombre);
            if (existe)
                return Results.Ok(new { existe = true, mensaje = "Tipo válido" });
            else
                return Results.NotFound(new { existe = false, mensaje = "Tipo no encontrado" });
        });
    }
}