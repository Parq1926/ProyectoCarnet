// UsuariosSRV4/Entities/UsuarioEntities.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UsuariosSRV4.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Contrasena { get; set; } = string.Empty;

        [Required]
        public int TipoIdentificacionId { get; set; }

        [Required]
        public string NumeroIdentificacion { get; set; } = string.Empty;

        [Required]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public int TipoUsuarioId { get; set; }

        public bool Activo { get; set; } = true;
        public bool Bloqueado { get; set; } = false;
        public int IntentosFallidos { get; set; } = 0;  // ← NUEVA PROPIEDAD
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaModificacion { get; set; }

        [JsonIgnore]
        public ICollection<Telefono>? Telefonos { get; set; }
    }

    public class Telefono
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}