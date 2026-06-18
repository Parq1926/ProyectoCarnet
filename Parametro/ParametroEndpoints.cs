using Microsoft.AspNetCore.Mvc;
using SRV15_Parametro.Auth;
using SRV15_Parametro.Entities;
using SRV15_Parametro.Services;

namespace SRV15_Parametro
{
    public static class ParametroEndpoints
    {
        public static void MapParametroEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/parametro")
                .WithTags("Parametro")
                .RequireCors("ReactDev");

            // GET /parametro - Obtener todos
            group.MapGet("/", async (HttpContext context, [FromServices] IParametroService service) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();
                return Results.Ok(await service.GetAllAsync());
            });

            // GET /parametro/{id} - Obtener por ID
            group.MapGet("/{id}", async (HttpContext context, string id, [FromServices] IParametroService service) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();
                var parametro = await service.GetByIdAsync(id);
                return parametro is null ? Results.NotFound() : Results.Ok(parametro);
            });

            // POST /parametro - Crear
            group.MapPost("/", async (HttpContext context, [FromBody] ParametroRequest request, [FromServices] IParametroService service) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();
                var (data, status) = await service.CreateAsync(request);
                // Exito: 201 con el parametro creado. Error: solo el codigo.
                return status == 201 ? Results.Created($"/parametro/{data!.Id}", data) : Results.StatusCode(status);
            });

            // PUT /parametro/{id} - Modificar
            group.MapPut("/{id}", async (HttpContext context, string id, [FromBody] ParametroRequest request, [FromServices] IParametroService service) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();
                var (data, status) = await service.UpdateAsync(id, request);
                // Exito: 200 con el parametro actualizado. Error: solo el codigo.
                return status == 200 ? Results.Ok(data) : Results.StatusCode(status);
            });

            // DELETE /parametro/{id} - Eliminar
            group.MapDelete("/{id}", async (HttpContext context, string id, [FromServices] IParametroService service) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();
                var (data, status) = await service.DeleteAsync(id);
                // Exito: 200 con el parametro eliminado. No existe: 404 sin cuerpo.
                return status == 200 ? Results.Ok(data) : Results.StatusCode(status);
            });
        }
    }
}
