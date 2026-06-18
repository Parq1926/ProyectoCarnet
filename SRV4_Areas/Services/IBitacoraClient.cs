namespace SRV4_Areas.Services;

public interface IBitacoraClient
{
    Task RegistrarAsync(
        string usuario,
        string accion,
        string token);
}