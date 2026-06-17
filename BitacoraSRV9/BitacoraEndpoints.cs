using BitacoraSRV9.Auth;
using BitacoraSRV9.Entities;
using BitacoraSRV9.Services;

namespace BitacoraSRV9;

public static class BitacoraEndpoints
{
    public static void MapBitacoraEndpoints(
        this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/bitacora",
        async (
            HttpContext context,
            BitacoraRequest request,
            IBitacoraService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator =
                context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var resultado =
                await service.RegistrarAsync(request);

            if (!resultado.ok)
            {
                return Results.BadRequest(new
                {
                    mensaje = resultado.error,
                    usuario = request.Usuario,
                    accion = request.Accion
                });
            }

            return Results.Ok(new
            {
                mensaje = "Movimiento registrado correctamente",
                usuario = request.Usuario,
                accion = request.Accion,
                fecha = DateTime.Now
            });
        });

        routes.MapGet("/bitacora",
        async (
            HttpContext context,
            IBitacoraService service) =>
        {
            var token = context.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            var tokenValidator =
                context.RequestServices
                .GetRequiredService<ITokenValidator>();

            if (!await tokenValidator.ValidateAsync(token))
                return Results.Unauthorized();

            var lista =
                await service.ObtenerTodosAsync();

            return Results.Ok(lista);
        });
    }
}