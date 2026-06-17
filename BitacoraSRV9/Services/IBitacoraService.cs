using BitacoraSRV9.Entities;

namespace BitacoraSRV9.Services;

public interface IBitacoraService
{
    Task<(bool ok, string error)> RegistrarAsync(
        BitacoraRequest request);

    Task<IEnumerable<Bitacora>> ObtenerTodosAsync();
}