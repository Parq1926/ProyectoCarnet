using SRV4_Areas.Auth;
using SRV4_Areas.Services;

namespace SRV4_Areas;

public static class AreaEndpoint
{
    public static void MapAreaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/areas");

        group.MapGet("/", async (
            HttpContext context,
            IAreaService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var areas = await service.GetAll();

            return Results.Ok(areas);
        });

        group.MapGet("/{id}", async (
            HttpContext context,
            int id,
            IAreaService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var area = await service.GetById(id);

            if (area != null)
                return Results.Ok(area);

            return Results.NotFound($"Area con ID {id} no encontrada");
        });

        group.MapPost("/", async (
            HttpContext context,
            CreateAreaRequest request,
            IAreaService service,
            IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var result = await service.Create(request);

            if (result.success)
            {
                await bitacoraClient.RegistrarAsync(
                    "Administrador del Sistema",
                    $"Creó el area {request.Nombre}",
                    token);

                return Results.Created($"/areas/{result.id}", new
                {
                    message = result.message,
                    area = new
                    {
                        id = result.id,
                        nombre = request.Nombre,
                        institucionID = request.InstitucionID,
                        institucionNombre = request.InstitucionNombre
                    }
                });
            }

            return Results.BadRequest(new
            {
                error = result.message
            });
        });

        group.MapPut("/{id}", async (
            HttpContext context,
            int id,
            UpdateAreaRequest request,
            IAreaService service,
            IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            if (id != request.ID)
            {
                return Results.BadRequest(new
                {
                    error = "El ID de la ruta no coincide con el ID del cuerpo"
                });
            }

            var result = await service.Update(request);

            if (result.success)
            {
                await bitacoraClient.RegistrarAsync(
                    "Administrador del Sistema",
                    $"Modificó el area {request.Nombre}",
                    token);

                return Results.Ok(new
                {
                    message = result.message,
                    area = new
                    {
                        id = request.ID,
                        nombre = request.Nombre,
                        institucionID = request.InstitucionID,
                        institucionNombre = request.InstitucionNombre
                    }
                });
            }

            return Results.BadRequest(new
            {
                error = result.message
            });
        });

        group.MapDelete("/{id}", async (
            HttpContext context,
            int id,
            IAreaService service,
            IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var area = await service.GetById(id);

            if (area == null)
            {
                return Results.NotFound(new
                {
                    error = $"Area con ID {id} no encontrada"
                });
            }

            var result = await service.Delete(id);

            if (result.success)
            {
                await bitacoraClient.RegistrarAsync(
                    "Administrador del Sistema",
                    $"Eliminó el area {area.Nombre}",
                    token);

                return Results.Ok(new
                {
                    message = result.message,
                    area = new
                    {
                        id = area.ID,
                        nombre = area.Nombre,
                        institucionID = area.InstitucionID,
                        institucionNombre = area.InstitucionNombre
                    }
                });
            }

            return Results.BadRequest(new
            {
                error = result.message
            });
        });
    }
}