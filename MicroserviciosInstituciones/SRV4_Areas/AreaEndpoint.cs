using SRV4_Areas.Services;

namespace SRV4_Areas;

public static class AreaEndpoint
{
    public static void MapAreaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/areas");

        group.MapGet("/", async (IAreaService service) =>
        {
            var areas = await service.GetAll();
            return Results.Ok(areas);
        });

        group.MapGet("/{id}", async (int id, IAreaService service) =>
        {
            var area = await service.GetById(id);
            if (area != null)
            {
                return Results.Ok(area);
            }
            return Results.NotFound($"Area con ID {id} no encontrada");
        });

        group.MapPost("/", async (CreateAreaRequest request, IAreaService service) =>
        {
            var result = await service.Create(request);
            if (result.success)
            {
                return Results.Created($"/areas/{result.id}", new { id = result.id, message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });

        group.MapPut("/{id}", async (int id, UpdateAreaRequest request, IAreaService service) =>
        {
            if (id != request.ID)
            {
                return Results.BadRequest(new { error = "El ID de la ruta no coincide con el ID del cuerpo" });
            }

            var result = await service.Update(request);
            if (result.success)
            {
                return Results.Ok(new { message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });

        group.MapDelete("/{id}", async (int id, IAreaService service) =>
        {
            var result = await service.Delete(id);
            if (result.success)
            {
                return Results.Ok(new { message = result.message });
            }
            return Results.BadRequest(new { error = result.message });
        });
    }
}