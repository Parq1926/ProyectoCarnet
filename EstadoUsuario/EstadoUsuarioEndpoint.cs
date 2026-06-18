using SRV12_EstadoUsuario.Auth;
using SRV12_EstadoUsuario.Entities;
using SRV12_EstadoUsuario.Services;
using Microsoft.AspNetCore.Mvc;

namespace SRV12_EstadoUsuario
{
    public static class EstadoUsuarioEndpoints
    {
        public static void MapEstadoUsuarioEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/usuarios/estado")
                .WithTags("EstadoUsuario")
                .RequireCors("ReactDev");

            // PATCH /usuarios/estado - Cambiar estado de un usuario (SRV12)
            group.MapMethods("/", new[] { "PATCH" }, async (
                HttpContext context,
                [FromServices] IEstadoUsuarioService service,
                [FromBody] CambioEstadoRequest request) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();

                if (string.IsNullOrWhiteSpace(request.UsuarioIdentificacion) || string.IsNullOrWhiteSpace(request.CodigoEstado))
                    return Results.BadRequest();

                var (data, status) = await service.CambiarEstadoAsync(request.UsuarioIdentificacion, request.CodigoEstado);

                // Exito: 200 con el estado actualizado. Error: solo el codigo.
                return status == 200 ? Results.Ok(data) : Results.StatusCode(status);
            })
            .WithName("CambiarEstadoUsuario");
        }
    }
}
