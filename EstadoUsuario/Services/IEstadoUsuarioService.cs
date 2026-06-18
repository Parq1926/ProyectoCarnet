using SRV12_EstadoUsuario.Entities;

namespace SRV12_EstadoUsuario.Services
{
    public interface IEstadoUsuarioService
    {
        // status: 200 ok | 404 estado inexistente | 500 fallo
        Task<(EstadoUsuarioResponse? data, int status)> CambiarEstadoAsync(string identificacion, string codigoEstado);
    }
}
