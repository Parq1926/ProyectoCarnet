using Microsoft.AspNetCore.Mvc;
using SRV15_Parametro.Entities;
using SRV15_Parametro.Services;

namespace SRV15_Parametro
{
    public static class ParametroEndpoints
    {
        public static void MapParametroEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/parametro")
                .WithTags("Parametro")
                .RequireCors("ReactDev");

            // GET /api/parametro - Obtener todos
            group.MapGet("/", async ([FromServices] IParametroService service) =>
            {
                // --- AUTENTICACIÓN (descomentar cuando SRV1 esté disponible) ---
                // var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                // var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                // if (!await tokenValidator.ValidateAsync(token))
                //     return Results.Unauthorized();
                // ------------------------------------------------------------------

                var lista = await service.GetAllAsync();
                return Results.Ok(lista);
            })
            .WithName("GetAllParametros");

            // GET /api/parametro/{id} - Obtener por ID
            group.MapGet("/{id}", async (
                string id,
                [FromServices] IParametroService service) =>
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Results.BadRequest(new { message = "El identificador es requerido" });

                var parametro = await service.GetByIdAsync(id);
                return parametro is null
                    ? Results.NotFound(new { message = $"No se encontró el parámetro '{id}'" })
                    : Results.Ok(parametro);
            })
            .WithName("GetParametroById");

            // POST /api/parametro - Crear
            group.MapPost("/", async (
                [FromBody] ParametroRequest request,
                [FromServices] IParametroService service) =>
            {
                var (ok, error) = await service.CreateAsync(request);
                return ok
                    ? Results.Created($"/api/parametro/{request.Id}", new { message = "Parámetro creado correctamente" })
                    : Results.BadRequest(new { message = error });
            })
            .WithName("CreateParametro");

            // PUT /api/parametro/{id} - Modificar
            group.MapPut("/{id}", async (
                string id,
                [FromBody] ParametroRequest request,
                [FromServices] IParametroService service) =>
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Results.BadRequest(new { message = "El identificador es requerido" });

                var (ok, error) = await service.UpdateAsync(id, request);
                if (!ok && error.Contains("No se encontró"))
                    return Results.NotFound(new { message = error });

                return ok
                    ? Results.Ok(new { message = "Parámetro actualizado correctamente" })
                    : Results.BadRequest(new { message = error });
            })
            .WithName("UpdateParametro");

            // DELETE /api/parametro/{id} - Eliminar
            group.MapDelete("/{id}", async (
                string id,
                [FromServices] IParametroService service) =>
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Results.BadRequest(new { message = "El identificador es requerido" });

                var (ok, error) = await service.DeleteAsync(id);
                return ok
                    ? Results.Ok(new { message = "Parámetro eliminado correctamente" })
                    : Results.NotFound(new { message = error });
            })
            .WithName("DeleteParametro");
        }
    }
}
