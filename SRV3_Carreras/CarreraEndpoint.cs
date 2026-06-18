using SRV3_Carreras.Auth;
using SRV3_Carreras.Services;

namespace SRV3_Carreras;

public static class CarreraEndpoint
{
    public static void MapCarreraEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/carreras");

        group.MapGet("/", async (
            HttpContext context,
            ICarreraService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var carreras = await service.GetAll();

            return Results.Ok(carreras);
        });

        group.MapGet("/{id}", async (
            HttpContext context,
            int id,
            ICarreraService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var carrera = await service.GetById(id);

            if (carrera != null)
            {
                return Results.Ok(carrera);
            }

            return Results.NotFound($"Carrera con ID {id} no encontrada");
        });

        group.MapPost("/", async (
            HttpContext context,
            CreateCarreraRequest request,
            ICarreraService service,
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
                    $"Creó la carrera {request.Nombre}",
                    token);

                return Results.Created($"/carreras/{result.id}", new
                {
                    message = result.message,
                    carrera = new
                    {
                        id = result.id,
                        nombre = request.Nombre,
                        director = request.Director,
                        email = request.Email,
                        telefono = request.Telefono,
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
            UpdateCarreraRequest request,
            ICarreraService service,
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
                    $"Modificó la carrera {request.Nombre}",
                    token);

                return Results.Ok(new
                {
                    message = result.message,
                    carrera = new
                    {
                        id = request.ID,
                        nombre = request.Nombre,
                        director = request.Director,
                        email = request.Email,
                        telefono = request.Telefono,
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
            ICarreraService service,
            IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var carrera = await service.GetById(id);

            if (carrera == null)
            {
                return Results.NotFound(new
                {
                    error = $"Carrera con ID {id} no encontrada"
                });
            }

            var result = await service.Delete(id);

            if (result.success)
            {
                await bitacoraClient.RegistrarAsync(
                    "Administrador del Sistema",
                    $"Eliminó la carrera {carrera.Nombre}",
                    token);

                return Results.Ok(new
                {
                    message = result.message,
                    carrera = new
                    {
                        id = carrera.ID,
                        nombre = carrera.Nombre,
                        director = carrera.Director,
                        email = carrera.Email,
                        telefono = carrera.Telefono,
                        institucionID = carrera.InstitucionID,
                        institucionNombre = carrera.InstitucionNombre
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