namespace SRV6_TipoIdentificacion.DTOs;

public class TipoIdentificacionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CrearTipoIdentificacionDto
{
    public string Nombre { get; set; } = string.Empty;
}