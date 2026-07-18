<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
using SRV6_TipoIdentificacion.DTOs;
using SRV6_TipoIdentificacion.Interfaces;

namespace SRV6_TipoIdentificacion.Endpoints;

public static class TipoIdentificacionEndpoints
{
    public static void MapTipoIdentificacionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/TipoIdentificacion")
            .WithTags("TipoIdentificacion");


        // GET /api/TipoIdentificacion 
        // Público para cargar datos en Autoregistro (SOLO ESTE ES EL QUE ESTA PUBLICO, LOS DEMAS NECESITAN AUTORIZACION)
        group.MapGet("/", async (ITipoIdentificacionService service) =>
        {
            var tipos = await service.GetAllAsync();
            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipos });
        });


        // GET /api/TipoIdentificacion/{id} 
        // Requiere autenticación
        group.MapGet("/{id}", async (int id, ITipoIdentificacionService service) =>
        {
            var tipo = await service.GetByIdAsync(id);

            if (tipo == null)
                return Results.NotFound(new { codigo = 404, mensaje = $"Tipo de identificación con ID {id} no encontrado" });

            return Results.Ok(new { codigo = 200, mensaje = "OK", data = tipo });
        })
        .RequireAuthorization();


        // POST /api/TipoIdentificacion 
        // Requiere autenticación
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

        })
        .RequireAuthorization();


        // PUT /api/TipoIdentificacion/{id}
        // Requiere autenticación
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

        })
        .RequireAuthorization();


        // DELETE /api/TipoIdentificacion/{id}
        // Requiere autenticación
        group.MapDelete("/{id}", async (int id, ITipoIdentificacionService service) =>
        {
            var (ok, error) = await service.DeleteAsync(id);

            if (!ok)
                return Results.NotFound(new { codigo = 404, mensaje = error });

            return Results.Ok(new { codigo = 200, mensaje = error });

        })
        .RequireAuthorization();
=======
﻿using Microsoft.EntityFrameworkCore;
using TipoIdentificacionSRV6.Data;
using TipoIdentificacionSRV6.DTOs;
using TipoIdentificacionSRV6.Entities;

namespace TipoIdentificacionSRV6.Endpoints
{
    public static class TipoIdentificacionEndpoints
    {
        public static void MapTipoIdentificacionEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/TipoIdentificacion");

            // GET: api/TipoIdentificacion
            group.MapGet("/", async (ApplicationDbContext db) =>
            {
                var tipos = await db.TiposIdentificacion
                    .Select(t => new TipoIdentificacionDto
                    {
                        Id = t.Id,
                        Nombre = t.Nombre
                    })
                    .ToListAsync();
                return Results.Ok(tipos);
            });

            // GET: api/TipoIdentificacion/{id}
            group.MapGet("/{id}", async (int id, ApplicationDbContext db) =>
            {
                var tipo = await db.TiposIdentificacion.FindAsync(id);
                if (tipo == null)
                    return Results.NotFound();

                return Results.Ok(new TipoIdentificacionDto
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre
                });
            });

            // POST: api/TipoIdentificacion
            group.MapPost("/", async (TipoIdentificacionCreateDto dto, ApplicationDbContext db) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return Results.BadRequest(new { error = "El nombre es requerido" });

                var tipo = new TipoIdentificacion
                {
                    Nombre = dto.Nombre.Trim()
                };

                db.TiposIdentificacion.Add(tipo);
                await db.SaveChangesAsync();

                return Results.Created($"/api/TipoIdentificacion/{tipo.Id}", new TipoIdentificacionDto
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre
                });
            });

            // PUT: api/TipoIdentificacion/{id}
            group.MapPut("/{id}", async (int id, TipoIdentificacionUpdateDto dto, ApplicationDbContext db) =>
            {
                var tipo = await db.TiposIdentificacion.FindAsync(id);
                if (tipo == null)
                    return Results.NotFound();

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return Results.BadRequest(new { error = "El nombre es requerido" });

                tipo.Nombre = dto.Nombre.Trim();
                await db.SaveChangesAsync();

                return Results.Ok(new TipoIdentificacionDto
                {
                    Id = tipo.Id,
                    Nombre = tipo.Nombre
                });
            });

            // DELETE: api/TipoIdentificacion/{id}
            group.MapDelete("/{id}", async (int id, ApplicationDbContext db) =>
            {
                var tipo = await db.TiposIdentificacion.FindAsync(id);
                if (tipo == null)
                    return Results.NotFound();

                db.TiposIdentificacion.Remove(tipo);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
>>>>>>> a7a79ac (Actualizacion del Login)
    }
}