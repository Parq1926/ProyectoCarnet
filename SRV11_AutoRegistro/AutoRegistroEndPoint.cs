using SRV11_AutoRegistro.Services;

namespace SRV11_Autoregistro;

public static class AutoregistroEndpoint
{
    public static void MapAutoregistroEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/autoregistro");

        group.MapPost("/", async (
            CreateUsuarioRequest request,
            IUsuarioService service,
            BitacoraService bitacora) =>
        {
            var result = await service.Registrar(request);

            if (result.success)
            {
                await bitacora.Registrar(
                    "Autoregistro",
                    $"Se registró el usuario {request.Email}");

                return Results.Created(
                    "/autoregistro",
                    new
                    {
                        message = result.message,
                        token = result.token
                    });
            }

            return Results.BadRequest(new
            {
                error = result.message
            });
        });

        group.MapGet("/confirmar/{token}", async (
            string token,
            IUsuarioService service,
            BitacoraService bitacora) =>
        {
            var result =
                await service.ConfirmarCuenta(token);

            if (result.success)
            {
                await bitacora.Registrar(
                    "Autoregistro",
                    "Cuenta confirmada");

                return Results.Ok(new
                {
                    message = result.message
                });
            }

            return Results.BadRequest(new
            {
                error = result.message
            });
        });
    }
}