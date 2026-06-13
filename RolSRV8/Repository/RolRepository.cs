using RolSRV8.Data;
using RolSRV8.Entities;

namespace RolSRV8.Repository;

public class RolRepository
{
    private readonly RolDbContext _context;

    public RolRepository(RolDbContext context)
    {
        _context = context;
    }

    public async Task Guardar(Rol rol)
    {
        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();
    }

    public List<Rol> ObtenerTodos()
    {
        return _context.Roles.ToList();
    }

    public Rol? ObtenerPorId(int id)
    {
        return _context.Roles.FirstOrDefault(x => x.Id == id);
    }

    public async Task Actualizar()
    {
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(Rol rol)
    {
        _context.Roles.Remove(rol);
        await _context.SaveChangesAsync();
    }
}