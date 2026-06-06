namespace LoginSRV1.Models
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
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public TipoUsuario? TipoUsuario { get; set; }
        public TipoIdentificacion? TipoIdentificacion { get; set; }

    }
}
