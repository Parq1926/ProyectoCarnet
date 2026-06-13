using SRV11_AutoRegistro.Entities;
using SRV11_AutoRegistro.Services;
using SRV11_AutoRegistro.Repository;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Json;


namespace SRV11_AutoRegistro.Services;

public class UsuarioService : IUsuarioService
{
    private readonly UsuarioRepository _repository;
    private readonly IConfiguration _configuration;

    public UsuarioService(
        UsuarioRepository repository,
        IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();

        var hash =
            sha.ComputeHash(
                Encoding.UTF8.GetBytes(password));

        return Convert.ToBase64String(hash);
    }

    private async Task<bool> ValidarDominio(string email)
    {
        try
        {
            using var httpClient = new HttpClient();

            var instituciones =
                await httpClient.GetFromJsonAsync<List<Institucion>>
                (
                    "http://localhost:7002/institucion"
                );

            if (instituciones == null)
                return false;

            var dominioCorreo =
                email.Split('@')[1].ToLower();

            foreach (var institucion in instituciones)
            {
                var dominios =
                    institucion.Dominios.Split(',');

                foreach (var dominio in dominios)
                {
                    if (dominio.Trim().ToLower() == dominioCorreo)
                        return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool success, string message, string? token)>
        Registrar(CreateUsuarioRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return (false, "El email es requerido", null);

        if (!IsValidEmail(request.Email))
            return (false, "El email no es valido", null);

        if (string.IsNullOrWhiteSpace(request.NombreCompleto))
            return (false,
                "El nombre completo es requerido",
                null);

        if (string.IsNullOrWhiteSpace(request.Identificacion))
            return (false,
                "La identificacion es requerida",
                null);

        if (string.IsNullOrWhiteSpace(request.Password))
            return (false,
                "La contraseña es requerida",
                null);

        if (await _repository.ExistsByEmail(request.Email))
            return (false,
                "Ya existe un usuario con ese correo",
                null);

        var minutos =
            _configuration.GetValue<int>(
                "TokenExpirationMinutes",
                15);

        var token = Guid.NewGuid().ToString();

        var usuario = new Usuario
        {
            Email = request.Email.Trim(),
            TipoIdentificacionID =
                request.TipoIdentificacionID,
            Identificacion =
                request.Identificacion.Trim(),
            NombreCompleto =
                request.NombreCompleto.Trim(),
            PasswordHash =
                HashPassword(request.Password),
            TipoUsuarioID =
                request.TipoUsuarioID,
            RolID =
                request.RolID,
            Confirmado = false,
            TokenConfirmacion = token,
            FechaExpiracion =
                DateTime.Now.AddMinutes(minutos),
            Activo = true
        };

        await _repository.Create(usuario);

        return (
            true,
            "Usuario registrado exitosamente",
            token
        );
    }

    public async Task<(bool success, string message)>
        ConfirmarCuenta(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return (false, "Token invalido");

        var usuario =
            await _repository.GetByToken(token);

        if (usuario == null)
            return (false, "Token no encontrado");

        if (usuario.FechaExpiracion < DateTime.Now)
            return (false, "El token ha expirado");

        var confirmado =
            await _repository.ConfirmarCuenta(token);

        return confirmado
            ? (true, "Cuenta confirmada correctamente")
            : (false, "No se pudo confirmar la cuenta");
    }
}