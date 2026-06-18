using SRV12_EstadoUsuario.Entities;
using SRV12_EstadoUsuario.Repository;

namespace SRV12_EstadoUsuario.Services
{
    public class EstadoUsuarioService : IEstadoUsuarioService
    {
        private readonly EstadoUsuarioRepository _repository;
        public EstadoUsuarioService(EstadoUsuarioRepository repository) { _repository = repository; }

        public async Task<(EstadoUsuarioResponse? data, int status)> CambiarEstadoAsync(string identificacion, string codigoEstado)
        {
            var estado = await _repository.GetEstadoByCodigoAsync(codigoEstado);
            if (estado is null) return (null, 404);

            var rows = await _repository.UpsertEstadoUsuarioAsync(identificacion, estado.Id);
            if (rows <= 0) return (null, 500);

            var data = new EstadoUsuarioResponse
            {
                UsuarioIdentificacion = identificacion,
                CodigoEstado = estado.Codigo,
                Descripcion = estado.Descripcion
            };
            return (data, 200);
        }
    }
}
