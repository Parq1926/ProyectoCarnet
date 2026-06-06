namespace TipoUsuarioSRV5.DTOs
{
    public class TipoUsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class CrearTipoUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
    }
}