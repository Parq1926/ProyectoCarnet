using Microsoft.AspNetCore.Mvc;
using RolSRV8.Auth;
using RolSRV8.Entities;
using RolSRV8.Services;

namespace RolSRV8;

public static class RolEndpoints
{
    public static void MapRolEndpoints(
    this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/rol",
        async (
        HttpContext context,
        RolRequest request,
        IRolService service,
        IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
    .ToString()
    .Replace("Bearer ", "");

        var tokenValidator =
                    context.RequestServices
                    .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var resultado =
        await service.CrearAsync(request);

            if (!resultado.ok)
            {
                return Results.BadRequest(new
                {
                    mensaje = resultado.error
                });
            }

            await bitacoraClient.RegistrarAsync(
        "Administrador del Sistema",
        $"Creó el rol {request.Nombre}",
        token);

            return Results.Ok(new
            {
                mensaje = "Rol creado correctamente",
                nombre = request.Nombre,
                pantallas = request.Pantallas
            });
        });

        routes.MapGet("/rol",
        async (
            HttpContext context,
            IRolService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator =
                context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var lista =
                await service.ObtenerTodosAsync();

            return Results.Ok(lista);
        });

        routes.MapGet("/rol/{id}",
        async (
            HttpContext context,
            int id,
            IRolService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator =
                context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var rol =
                await service.ObtenerPorIdAsync(id);

            if (rol == null)
            {
                return Results.NotFound(new
                {
                    mensaje = "Rol no encontrado"
                });
            }

            return Results.Ok(rol);
        });

        routes.MapPut("/rol/{id}",
        async (
            HttpContext context,
            int id,
            RolRequest request,
            IRolService service,
            IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator =
                context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var resultado =
                await service.ActualizarAsync(
                    id,
                    request);

            if (!resultado.ok)
            {
                return Results.BadRequest(new
                {
                    mensaje = resultado.error
                });
            }

            await bitacoraClient.RegistrarAsync(
                "Administrador del Sistema",
                $"Modificó el rol {request.Nombre}",
                token);

            return Results.Ok(new
            {
                mensaje = "Rol actualizado correctamente",
                id = id,
                nombre = request.Nombre,
                pantallas = request.Pantallas
            });
        });

        routes.MapDelete("/rol/{id}",
        async (
            HttpContext context,
            int id,
            IRolService service,
            IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator =
                context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var rol =
                await service.ObtenerPorIdAsync(id);

            if (rol == null)
            {
                return Results.NotFound(new
                {
                    mensaje = "Rol no encontrado"
                });
            }

            var resultado =
                await service.EliminarAsync(id);

            if (!resultado.ok)
            {
                return Results.BadRequest(new
                {
                    mensaje = resultado.error
                });
            }

            await bitacoraClient.RegistrarAsync(
                "Administrador del Sistema",
                $"Eliminó el rol {rol.Nombre}",
                token);

            return Results.Ok(new
            {
                mensaje = "Rol eliminado correctamente",
                id = rol.Id,
                nombre = rol.Nombre,
                pantallas = rol.Pantallas
            });
        });
    }

}
