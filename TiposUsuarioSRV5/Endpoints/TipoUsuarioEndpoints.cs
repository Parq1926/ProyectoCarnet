<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
using SRV5_TipoUsuario.Interfaces;
using SRV5_TipoUsuario.DTOs;

namespace SRV5_TipoUsuario.Endpoints;

public static class TipoUsuarioEndpoints
{
    public static void MapTipoUsuarioEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TipoUsuario")
            .WithTags("TipoUsuario");

        // GET /api/TipoUsuario - Listar todos
        // Público para cargar datos en Autoregistro
        group.MapGet("/", async (ITipoUsuarioService service) =>
        {
            var tipos = await service.GetAllAsync();
            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipos });
        });

        // GET /api/TipoUsuario/{id} - Obtener por ID
        // Requiere autenticación
        group.MapGet("/{id}", async (int id, ITipoUsuarioService service) =>
        {
            var tipo = await service.GetByIdAsync(id);

            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de usuario con ID {id} no encontrado" });

            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipo });

        })
        .RequireAuthorization();

        // POST /api/TipoUsuario - Crear nuevo
        // Requiere autenticación
        group.MapPost("/", async ([FromBody] CrearTipoUsuarioDto dto, ITipoUsuarioService service) =>
        {
            var (ok, error, data) = await service.CreateAsync(dto);

            if (!ok)
            {
                if (error.Contains("ya existe"))
                    return Results.Conflict(new { codigo = 409, mensaje = error });

                return Results.BadRequest(new { codigo = 400, mensaje = error });
            }

            return Results.Created($"/api/TipoUsuario/{data?.Id}", new
            {
                codigo = 201,
                mensaje = "Tipo de usuario creado exitosamente",
                data = data
            });

        })
        .RequireAuthorization();

        // PUT /api/TipoUsuario/{id} - Actualizar
        // Requiere autenticación
        group.MapPut("/{id}", async (int id, [FromBody] CrearTipoUsuarioDto dto, ITipoUsuarioService service) =>
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
                mensaje = $"Tipo de usuario actualizado exitosamente. {error}",
                anterior = error.Split("'")[1] ?? "",
                nuevo = error.Split("'")[3] ?? "",
                data = data
            });

        })
        .RequireAuthorization();

        // DELETE /api/TipoUsuario/{id} - Eliminar
        // Requiere autenticación
        group.MapDelete("/{id}", async (int id, ITipoUsuarioService service) =>
        {
            var (ok, error) = await service.DeleteAsync(id);

            if (!ok)
                return Results.NotFound(new { codigo = 404, mensaje = error });

            return Results.Ok(new { codigo = 200, mensaje = error });

        })
        .RequireAuthorization();

        // GET /api/TipoUsuario/validar/{nombre} - Validar existencia
        // Requiere autenticación
        group.MapGet("/validar/{nombre}", async (string nombre, ITipoUsuarioService service) =>
        {
            var existe = await service.ValidarExistenciaAsync(nombre);

            if (existe)
                return Results.Ok(new { existe = true, mensaje = "Tipo válido" });

            return Results.NotFound(new { existe = false, mensaje = "Tipo no encontrado"
        });

    })
        .RequireAuthorization();
}
=======
﻿using Microsoft.EntityFrameworkCore;
using TiposUsuarioSRV5.Data;
using TiposUsuarioSRV5.DTOs;
using TiposUsuarioSRV5.Entities;

namespace TiposUsuarioSRV5.Endpoints
{
    public static class TipoUsuarioEndpoints
    {
        public static void MapTipoUsuarioEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/TipoUsuario");

            // GET: api/TipoUsuario
            group.MapGet("/", async (ApplicationDbContext db) =>
            {
                var tipos = await db.TiposUsuario
                    .Select(t => new TipoUsuarioDto
                    {
                        Id = t.Id,
                        Nombre = t.Nombre
                    })
                    .ToListAsync();
                return Results.Ok(tipos);
            });

            // GET: api/TipoUsuario/{id}
            group.MapGet("/{id}", async (int id, ApplicationDbContext db) =>
            {
                var tipo = await db.TiposUsuario.FindAsync(id);
                if (tipo == null)
                    return Results.NotFound();

                return Results.Ok(new TipoUsuarioDto
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre
                });
            });

            // POST: api/TipoUsuario
            group.MapPost("/", async (TipoUsuarioCreateDto dto, ApplicationDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return Results.BadRequest(new { error = "El nombre es requerido" });

                var tipo = new TipoUsuario
                {
                    Nombre = dto.Nombre.Trim()
                };

                db.TiposUsuario.Add(tipo);
                await db.SaveChangesAsync();

                return Results.Created($"/api/TipoUsuario/{tipo.Id}", new TipoUsuarioDto
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre
                });
            });

            // PUT: api/TipoUsuario/{id}
            group.MapPut("/{id}", async (int id, TipoUsuarioUpdateDto dto, ApplicationDbContext db) =>
            {
                var tipo = await db.TiposUsuario.FindAsync(id);
                if (tipo == null)
                    return Results.NotFound();

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return Results.BadRequest(new { error = "El nombre es requerido" });

                tipo.Nombre = dto.Nombre.Trim();
                await db.SaveChangesAsync();

                return Results.Ok(new TipoUsuarioDto
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre
                });
            });

            // DELETE: api/TipoUsuario/{id}
            group.MapDelete("/{id}", async (int id, ApplicationDbContext db) =>
            {
                var tipo = await db.TiposUsuario.FindAsync(id);
                if (tipo == null)
                    return Results.NotFound();

                db.TiposUsuario.Remove(tipo);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
>>>>>>> a7a79ac (Actualizacion del Login)
}