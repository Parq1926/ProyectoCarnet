using BitacoraSRV9.Data;
using BitacoraSRV9.Entities;

namespace BitacoraSRV9.Repository;

public class BitacoraRepository
{
    private readonly BitacoraDbContext _context;

    public BitacoraRepository(BitacoraDbContext context)
    {
        _context = context;
    }

    public async Task Guardar(Bitacora bitacora)
    {
        _context.Bitacoras.Add(bitacora);
        await _context.SaveChangesAsync();
    }

    public List<Bitacora> ObtenerTodos()
    {
        return _context.Bitacoras
            .OrderByDescending(x => x.Fecha)
            .ToList();
    }
}