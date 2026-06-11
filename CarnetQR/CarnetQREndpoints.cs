using Microsoft.AspNetCore.Mvc;
using SRV14_CarnetQR.Services;

namespace SRV14_CarnetQR
{
    public static class CarnetQREndpoints
    {
        public static void MapCarnetQREndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes
                .MapGroup("/api/usuario/qr")
                .WithTags("CarnetQR")
                .RequireCors("ReactDev");

            // GET /api/usuario/qr?identificacion={id}  (SRV14)
            group.MapGet("/", async (
                [FromServices] ICarnetQRService service,
                [FromQuery] string identificacion) =>
            {
                if (string.IsNullOrWhiteSpace(identificacion))
                    return Results.BadRequest(new { message = "La identificación del usuario es requerida" });

                var qrBase64 = await service.GenerarQRAsync(identificacion);

                if (qrBase64 is null)
                    return Results.NotFound(new { message = $"No se encontró el usuario con identificación '{identificacion}'" });

                return Results.Ok(qrBase64);
            })
            .WithName("ObtenerCarnetQR")
            .WithSummary("Genera y devuelve el QR del carnet digital para un usuario");
        }
    }
}
