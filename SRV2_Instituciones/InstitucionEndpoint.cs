using SRV2_Instituciones.Services;
using SRV2_Instituciones.Auth;

namespace SRV2_Instituciones;

public static class InstitucionEndpoint
{
    public static void MapInstitucionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/institucion");

        group.MapGet("/", async (
            HttpContext context,
            IInstitucionService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var instituciones = await service.GetAll();
            return Results.Ok(instituciones);
        });

        group.MapGet("/{id}", async (
            HttpContext context,
            int id,
            IInstitucionService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var institucion = await service.GetById(id);

            if (institucion != null)
                return Results.Ok(institucion);

            return Results.NotFound($"Institucion con ID {id} no encontrada");
        });

        group.MapPost("/", async (
            HttpContext context,
            CreateInstitucionRequest request,
            IInstitucionService service,
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
                    $"Creó la institucion {request.Nombre}",
                    token);

                return Results.Created($"/institucion/{result.id}", new
                {
                    message = result.message,
                    institucion = new
                    {
                        id = result.id,
                        nombre = request.Nombre,
                        email = request.Email,
                        telefono = request.Telefono,
                        dominios = request.Dominios
                    }
                });
            }

            return Results.BadRequest(new { error = result.message });
        });

        group.MapPut("/{id}", async (
            HttpContext context,
            int id,
            UpdateInstitucionRequest request,
            IInstitucionService service,
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
                    $"Modificó la institucion {request.Nombre}",
                    token);

                return Results.Ok(new
                {
                    message = result.message,
                    institucion = new
                    {
                        id = request.ID,
                        nombre = request.Nombre,
                        email = request.Email,
                        telefono = request.Telefono,
                        dominios = request.Dominios
                    }
                });
            }

            return Results.BadRequest(new { error = result.message });
        });

        group.MapDelete("/{id}", async (
            HttpContext context,
            int id,
            IInstitucionService service,
            IBitacoraClient bitacoraClient) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator = context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var institucion = await service.GetById(id);

            if (institucion == null)
            {
                return Results.NotFound(new
                {
                    error = $"Institucion con ID {id} no encontrada"
                });
            }

            var result = await service.Delete(id);

            if (result.success)
            {
                await bitacoraClient.RegistrarAsync(
                    "Administrador del Sistema",
                    $"Eliminó la institucion {institucion.Nombre}",
                    token);

                return Results.Ok(new
                {
                    message = result.message,
                    institucion = new
                    {
                        id = institucion.ID,
                        nombre = institucion.Nombre,
                        email = institucion.Email,
                        telefono = institucion.Telefono,
                        dominios = institucion.Dominios
                    }
                });
            }

            return Results.BadRequest(new { error = result.message });
        });
    }
}