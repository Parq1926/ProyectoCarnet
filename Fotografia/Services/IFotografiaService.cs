using SRV13_Fotografia.Entities;

namespace SRV13_Fotografia.Services
{
    public interface IFotografiaService
    {
        Task<FotografiaUsuario?> ObtenerFotografiaAsync(string identificacion);
        // status: 200 ok | 422 Base64 invalido o mayor a 1MB | 500 fallo
        Task<(FotografiaUsuario? data, int status)> ActualizarFotografiaAsync(string identificacion, string fotografiaBase64);
        Task<FotografiaUsuario?> EliminarFotografiaAsync(string identificacion);
    }
}
