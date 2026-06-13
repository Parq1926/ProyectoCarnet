using BitacoraSRV9.Entities;
using BitacoraSRV9.Repository;

namespace BitacoraSRV9.Services;

public class BitacoraService
{
    private readonly BitacoraRepository _repository;

    public BitacoraService(BitacoraRepository repository)
    {
        _repository = repository;
    }

    public async Task Registrar(string usuario, string accion)
    {
        Bitacora bitacora = new Bitacora
        {
            Usuario = usuario,
            Accion = accion,
            Fecha = DateTime.Now
        };

        await _repository.Guardar(bitacora);
    }

    public List<Bitacora> ObtenerTodos()
    {
        return _repository.ObtenerTodos();
    }
}