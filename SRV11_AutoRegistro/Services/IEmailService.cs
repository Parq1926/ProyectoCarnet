using SRV11_AutoRegistro.Entities;

namespace SRV11_AutoRegistro.Services;

public interface IEmailService
{
    Task EnviarConfirmacion(
        string correo,
        string token);
}