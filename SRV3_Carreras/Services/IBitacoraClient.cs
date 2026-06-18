namespace SRV3_Carreras.Services;

public interface IBitacoraClient
{
    Task RegistrarAsync(
        string usuario,
        string accion,
        string token);
}