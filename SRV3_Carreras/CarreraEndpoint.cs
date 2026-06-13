using SRV3_Carreras.Services;

namespace SRV3_Carreras;

public static class CarreraEndpoint
{
    public static void MapCarreraEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/carreras");

        group.MapGet("/", async (ICarreraService service) =>
        {
            var carreras = await service.GetAll();
            return Results.Ok(carreras);
        });

        group.MapGet("/{id}", async (int id, ICarreraService service) =>
        {
            var carrera = await service.GetById(id);
            if (carrera != null)
            {
                return Results.Ok(carrera);
            }
            return Results.NotFound($"Carrera con ID {id} no encontrada");
        });

        group.MapPost("/", async (CreateCarreraRequest request, ICarreraService service, IBitacoraService bitacora) =>
        {
            var result = await service.Create(request);
            if (result.success)
            {
                await bitacora.Registrar("Administrador del Sistema", $"Creó la carrera {request.Nombre}");
                return Results.Created($"/carreras/{result.id}", new { id = result.id, message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });

        group.MapPut("/{id}", async (int id, UpdateCarreraRequest request, ICarreraService service, IBitacoraService bitacora) =>
        {
            if (id != request.ID)
            {
                return Results.BadRequest(new { error = "El ID de la ruta no coincide con el ID del cuerpo" });
            }

            var result = await service.Update(request);
            if (result.success)
            {
                await bitacora.Registrar("Administrador del Sistema", $"Modificó la carrera {request.Nombre}");
                return Results.Ok(new { message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });

        group.MapDelete("/{id}", async (int id, ICarreraService service, IBitacoraService bitacora) =>
        {
            var carrera = await service.GetById(id);
            var nombreCarrera = carrera?.Nombre ?? "Desconocida";

            var result = await service.Delete(id);
            if (result.success)
            {
                await bitacora.Registrar("Administrador del Sistema", $"Eliminó la carrera {nombreCarrera}");
                return Results.Ok(new { message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });
    }
}