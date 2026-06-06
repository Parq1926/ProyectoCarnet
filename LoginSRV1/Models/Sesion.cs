namespace LoginSRV1.Models
{
    public class Sesion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime FechaExpiracion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
