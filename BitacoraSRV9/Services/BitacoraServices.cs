using BitacoraSRV9.Entities;
using BitacoraSRV9.Repository;

namespace BitacoraSRV9.Services;

public class BitacoraService : IBitacoraService
{
    private readonly BitacoraRepository _repository;

    public BitacoraService(BitacoraRepository repository)
    {
        _repository = repository;
    }

    public async Task<(bool ok, string error)>
        RegistrarAsync(BitacoraRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario))
            return (false, "El usuario es requerido");

        if (string.IsNullOrWhiteSpace(request.Accion))
            return (false, "La acción es requerida");

        var bitacora = new Bitacora
        {
            Usuario = request.Usuario,
            Accion = request.Accion,
            Fecha = DateTime.Now
        };

        await _repository.GuardarAsync(bitacora);

        return (true, string.Empty);
    }

    public async Task<IEnumerable<Bitacora>>
        ObtenerTodosAsync()
    {
        return await _repository.ObtenerTodosAsync();
    }
}