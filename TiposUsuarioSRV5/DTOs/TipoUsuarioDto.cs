namespace SRV5_TipoUsuario.DTOs;

public class TipoUsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CrearTipoUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
}