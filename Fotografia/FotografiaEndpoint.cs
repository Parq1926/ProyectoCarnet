using SRV13_Fotografia.Auth;
using SRV13_Fotografia.Entities;
using SRV13_Fotografia.Services;
using Microsoft.AspNetCore.Mvc;

namespace SRV13_Fotografia
{
    public static class FotografiaEndpoints
    {
        public static void MapFotografiaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/usuario/fotografia")
                .WithTags("Fotografia")
                .RequireCors("ReactDev");

            // PUT /usuario/fotografia - Agregar o actualizar (upsert) la fotografia
            group.MapPut("/", async (
                HttpContext context,
                [FromServices] IFotografiaService service,
                [FromBody] ActualizarFotografiaRequest request) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();

                if (string.IsNullOrWhiteSpace(request.UsuarioIdentificacion) ||
                    string.IsNullOrWhiteSpace(request.FotografiaBase64))
                    return Results.BadRequest();

                var (data, status) = await service.ActualizarFotografiaAsync(
                    request.UsuarioIdentificacion, request.FotografiaBase64);

                // Exito: 200 con la fotografia guardada. 422 Base64 invalido o >1MB. 500 fallo. Sin cuerpo en errores.
                return status == 200 ? Results.Ok(data) : Results.StatusCode(status);
            })
            .WithName("ActualizarFotografia");

            // DELETE /usuario/fotografia/{identificacion}
            group.MapDelete("/{identificacion}", async (
                HttpContext context,
                [FromServices] IFotografiaService service,
                string identificacion) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();

                if (string.IsNullOrWhiteSpace(identificacion))
                    return Results.BadRequest();

                var deleted = await service.EliminarFotografiaAsync(identificacion);

                // Exito: 200 con la fotografia eliminada. No encontrada: 404 sin cuerpo.
                return deleted is null ? Results.NotFound() : Results.Ok(deleted);
            })
            .WithName("EliminarFotografia");

            // GET /usuario/fotografia/{identificacion}
            group.MapGet("/{identificacion}", async (
                HttpContext context,
                [FromServices] IFotografiaService service,
                string identificacion) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();

                var foto = await service.ObtenerFotografiaAsync(identificacion);

                // Exito: 200 con la fotografia. No encontrada: 404 sin cuerpo.
                return foto is null ? Results.NotFound() : Results.Ok(foto);
            })
            .WithName("ObtenerFotografia");
        }
    }
}
