using System.ComponentModel.DataAnnotations;

namespace UsuariosSRV4.DTOs
{
    // DTO para listar
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
        public int IntentosFallidos { get; set; }  // ← NUEVO
        public DateTime FechaCreacion { get; set; }
        public List<string> Telefonos { get; set; } = new();
    }

    // DTO para crear
    public class CrearUsuarioDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [RegularExpression(@"^[^@\s]+@(cuc\.cr|cuc\.ac\.cr)$",
            ErrorMessage = "Solo se permiten dominios cuc.cr o cuc.ac.cr")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identificación es requerido")]
        public int TipoIdentificacionId { get; set; }

        [Required(ErrorMessage = "El número de identificación es requerido")]
        [MinLength(5, ErrorMessage = "El número de identificación debe tener al menos 5 caracteres")]
        public string NumeroIdentificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [MinLength(3, ErrorMessage = "El nombre completo debe tener al menos 3 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de usuario es requerido")]
        public int TipoUsuarioId { get; set; }

        public List<string> Telefonos { get; set; } = new();
    }

    // DTO para actualizar
    public class ActualizarUsuarioDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [RegularExpression(@"^[^@\s]+@(cuc\.cr|cuc\.ac\.cr)$",
            ErrorMessage = "Solo se permiten dominios cuc.cr o cuc.ac.cr")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identificación es requerido")]
        public int TipoIdentificacionId { get; set; }

        [Required(ErrorMessage = "El número de identificación es requerido")]
        [MinLength(5, ErrorMessage = "El número de identificación debe tener al menos 5 caracteres")]
        public string NumeroIdentificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [MinLength(3, ErrorMessage = "El nombre completo debe tener al menos 3 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        public string? Contrasena { get; set; }

        [Required(ErrorMessage = "El tipo de usuario es requerido")]
        public int TipoUsuarioId { get; set; }

        public bool Activo { get; set; } = true;
        public List<string> Telefonos { get; set; } = new();
    }

    // DTO para validar credenciales (para LoginSRV1)
    public class ValidarCredencialesRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class ValidarCredencialesResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
    }

    // DTO para filtros
    public class FiltroUsuarioDto
    {
        public string? Identificacion { get; set; }
        public string? Nombre { get; set; }
        public string? TipoUsuario { get; set; }
    }



}