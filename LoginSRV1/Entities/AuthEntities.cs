// Entities/AuthEntities.cs
using System.Text.Json.Serialization;

namespace LoginSRV1.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int TipoUsuarioId { get; set; }      
        public int TipoIdentificacionId { get; set; } 
        public string NumeroIdentificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public bool Bloqueado { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [JsonIgnore]
        public ICollection<Sesion>? Sesiones { get; set; }
    }

    public class Sesion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }

    public class Parametro
    {
        public string Id { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}