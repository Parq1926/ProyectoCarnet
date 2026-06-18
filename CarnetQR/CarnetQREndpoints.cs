using Microsoft.AspNetCore.Mvc;
using SRV14_CarnetQR.Auth;
using SRV14_CarnetQR.Services;

namespace SRV14_CarnetQR
{
    public static class CarnetQREndpoints
    {
        public static void MapCarnetQREndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/usuario/qr")
                .WithTags("CarnetQR")
                .RequireCors("ReactDev");

            // GET /usuario/qr?identificacion={id}  (SRV14)
            group.MapGet("/", async (
                HttpContext context,
                [FromServices] ICarnetQRService service,
                [FromQuery] string identificacion) =>
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var tokenValidator = context.RequestServices.GetRequiredService<ITokenValidator>();
                if (!await tokenValidator.ValidateAsync(token))
                    return Results.Unauthorized();

                if (string.IsNullOrWhiteSpace(identificacion))
                    return Results.BadRequest();

                var qrBase64 = await service.GenerarQRAsync(identificacion);

                // Exito: 200 con el QR (Base64). Usuario inexistente: 404 sin cuerpo.
                return qrBase64 is null ? Results.NotFound() : Results.Ok(qrBase64);
            })
            .WithName("ObtenerCarnetQR")
            .WithSummary("Genera y devuelve el QR del carnet digital para un usuario");
        }
    }
}
