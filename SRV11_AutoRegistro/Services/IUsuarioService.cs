using SRV11_AutoRegistro.Entities;

namespace SRV11_AutoRegistro.Services;

public interface IUsuarioService
{
    Task<(bool success, string message, string? token)>
        Registrar(CreateUsuarioRequest request);

    Task<(bool success, string message)>
        ConfirmarCuenta(string token);
}

public class CreateUsuarioRequest
{
    public string Email { get; set; } = string.Empty;

    public int TipoIdentificacionID { get; set; }

    public string Identificacion { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int TipoUsuarioID { get; set; }

    public int RolID { get; set; }
}