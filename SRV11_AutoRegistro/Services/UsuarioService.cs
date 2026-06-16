using SRV11_AutoRegistro.Entities;
using SRV11_AutoRegistro.Repository;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;


namespace SRV11_AutoRegistro.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioRepository _repository;
        private readonly UsuarioCarreraRepository _carreraRepository;
        private readonly UsuarioAreaRepository _areaRepository;
        private readonly UsuarioInstitucionRepository _institucionRepository;
        private readonly IInstitucionService _institucionService;
        private readonly ICarreraService _carreraService;
        private readonly IAreaService _areaService;
        private readonly UsuarioTelefonoRepository _telefonoRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;


        public UsuarioService(
            UsuarioRepository repository,
            UsuarioCarreraRepository carreraRepository,
            UsuarioAreaRepository areaRepository,
            UsuarioInstitucionRepository institucionRepository,
            IInstitucionService institucionService,
            ICarreraService carreraService,
            IAreaService areaService,
            UsuarioTelefonoRepository telefonoRepository,
                IConfiguration configuration,
                    IEmailService emailService)
        {
            _repository = repository;
            _carreraRepository = carreraRepository;
            _areaRepository = areaRepository;
            _institucionRepository = institucionRepository;

            _institucionService = institucionService;
            _carreraService = carreraService;
            _areaService = areaService;
            _telefonoRepository = telefonoRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<(bool ok, string error, string token)> RegistrarAsync(Usuario usuario)
        {
            if (usuario.TipoUsuarioId <= 0)
                return (false, "Debe indicar un tipo de usuario", "");

            if (usuario.TipoIdentificacionId <= 0)
                return (false, "Debe indicar un tipo de identificación", "");

            if (string.IsNullOrWhiteSpace(usuario.NumeroIdentificacion))
                return (false, "La identificación es requerida", "");

            if (string.IsNullOrWhiteSpace(usuario.Email))
                return (false, "El email es requerido", "");

            if (!Regex.IsMatch(
                usuario.Email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return (false, "Formato de email inválido", "");
            }

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto))
                return (false, "El nombre completo es requerido", "");

            if (string.IsNullOrWhiteSpace(usuario.Contrasena))
                return (false, "La contraseña es requerida", "");

            // Validación estudiante
            if (usuario.TipoUsuarioId == 1)
            {
                if (usuario.CarrerasAsociadas == null ||
                    !usuario.CarrerasAsociadas.Any())
                {
                    return (false,
                        "El estudiante debe tener al menos una carrera",
                        "");
                }
            }

            // Validación funcionario
            if (usuario.TipoUsuarioId == 4)
            {
                if (usuario.AreasAsociadas == null ||
                    !usuario.AreasAsociadas.Any())
                {
                    return (false,
                        "El funcionario debe tener al menos un área",
                        "");
                }
            }

            var existe = await _repository.GetByEmailAsync(usuario.Email);

            if (existe is not null)
                return (false, "Ya existe un usuario con ese correo", "");


            if (!usuario.Instituciones.Any())
            {
                return (false,
                    "Debe seleccionar al menos una institución",
                    "");
            }

            var dominioCorreo = usuario.Email
                .Split('@')
                .Last()
                .Trim()
                .ToLower();

            var dominioValido = false;

            foreach (var institucionId in usuario.Instituciones)
            {
                var institucion =
                    await _institucionService.GetById(institucionId);

                if (institucion is null)
                {
                    return (
                        false,
                        $"La institución {institucionId} no existe",
                        ""
                    );
                }

                var dominios = institucion.Dominios
                    .Split(',')
                    .Select(x => x.Trim().ToLower());

                if (dominios.Contains(dominioCorreo))
                {
                    dominioValido = true;
                }
            }

            if (!dominioValido)
            {
                return (
                    false,
                    "El dominio del correo no pertenece a ninguna institución seleccionada",
                    ""
                );
            }

            if (usuario.TipoUsuarioId == 1)
            {
                foreach (var carreraId in usuario.CarrerasAsociadas)
                {
                    var carrera =
                        await _carreraService.GetById(carreraId);

                    if (carrera is null)
                    {
                        return (
                            false,
                            $"La carrera {carreraId} no existe",
                            ""
                        );
                    }
                }
            }

            if (usuario.TipoUsuarioId == 4)
            {
                foreach (var areaId in usuario.AreasAsociadas)
                {
                    var area =
                        await _areaService.GetById(areaId);

                    if (area is null)
                    {
                        return (
                            false,
                            $"El área {areaId} no existe",
                            ""
                        );
                    }
                }
            }

            var token = Guid.NewGuid().ToString();

            usuario.Contrasena =
                BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            usuario.Activo = true;
            usuario.Confirmado = false;
            usuario.FechaCreacion = DateTime.Now;
            usuario.TokenConfirmacion = token;
            var minutosExpiracion =
                _configuration.GetValue<int>("TokenExpirationMinutes");

            usuario.FechaExpiracion =
                DateTime.Now.AddMinutes(minutosExpiracion);

            var usuarioId = await _repository.CreateAsync(usuario);

            var enlaceConfirmacion =
                $"http://localhost:5221/autoregistro/confirmar/{token}";

            await _emailService.EnviarCorreoConfirmacionAsync(
                usuario.Email,
                enlaceConfirmacion);

            // Instituciones
            if (usuario.Instituciones != null)
            {
                foreach (var institucionId in usuario.Instituciones)
                {
                    await _institucionRepository
                        .AgregarAsync(usuarioId, institucionId);
                }
            }

            // Carreras
            if (usuario.CarrerasAsociadas != null)
            {
                foreach (var carreraId in usuario.CarrerasAsociadas)
                {
                    await _carreraRepository
                        .AgregarAsync(usuarioId, carreraId);
                }
            }

            // Áreas
            if (usuario.AreasAsociadas != null)
            {
                foreach (var areaId in usuario.AreasAsociadas)
                {
                    await _areaRepository
                        .AgregarAsync(usuarioId, areaId);
                }
            }

            // Teléfonos
            if (usuario.Telefonos != null)
            {
                foreach (var telefono in usuario.Telefonos)
                {
                    await _telefonoRepository
                        .AgregarAsync(usuarioId, telefono);
                }
            }

            return (true, string.Empty, token);
        }



        public async Task<(bool ok, string error)> ConfirmarCuentaAsync(string token)
        {
            var usuario = await _repository.GetByTokenAsync(token);

            if (usuario is null)
                return (false, "Token inválido");

            if (usuario.FechaExpiracion is null)
                return (false, "El token no tiene fecha de expiración");

            if (usuario.FechaExpiracion < DateTime.Now)
                return (false, "El token ha expirado");

            await _repository.ConfirmarCuentaAsync(usuario.ID);

            return (true, string.Empty);
        }
    }
}