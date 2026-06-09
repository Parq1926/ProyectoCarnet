namespace BitacoraSRV9.Entities;

public class Bitacora
{
    public int Id { get; set; }

    public string Usuario { get; set; } = string.Empty;

    public string Accion { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }
}