using SRV2_Instituciones.Services;

namespace SRV2_Instituciones;

public static class InstitucionEndpoint
{
    public static void MapInstitucionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/institucion");

        group.MapGet("/", async (IInstitucionService service) =>
        {
            var instituciones = await service.GetAll();
            return Results.Ok(instituciones);
        });

        group.MapGet("/{id}", async (int id, IInstitucionService service) =>
        {
            var institucion = await service.GetById(id);
            if (institucion != null)
            {
                return Results.Ok(institucion);
            }
            return Results.NotFound($"Institucion con ID {id} no encontrada");
        });

        group.MapPost("/", async (CreateInstitucionRequest request, IInstitucionService service, IBitacoraService bitacora) =>
        {
            var result = await service.Create(request);
            if (result.success)
            {
                await bitacora.Registrar("Administrador del Sistema", $"Creó la institucion {request.Nombre}");
                return Results.Created($"/institucion/{result.id}", new { id = result.id, message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });

        group.MapPut("/{id}", async (int id, UpdateInstitucionRequest request, IInstitucionService service, IBitacoraService bitacora) =>
        {
            if (id != request.ID)
            {
                return Results.BadRequest(new { error = "El ID de la ruta no coincide con el ID del cuerpo" });
            }

            var result = await service.Update(request);
            if (result.success)
            {
                await bitacora.Registrar("Administrador del Sistema", $"Modificó la institucion {request.Nombre}");
                return Results.Ok(new { message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });

        group.MapDelete("/{id}", async (int id, IInstitucionService service, IBitacoraService bitacora) =>
        {
            var institucion = await service.GetById(id);
            var nombreInstitucion = institucion?.Nombre ?? "Desconocida";

            var result = await service.Delete(id);
            if (result.success)
            {
                await bitacora.Registrar("Administrador del Sistema", $"Eliminó la institucion {nombreInstitucion}");
                return Results.Ok(new { message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });
    }
}