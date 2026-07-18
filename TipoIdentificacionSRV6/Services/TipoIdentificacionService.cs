using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using SRV6_TipoIdentificacion.Data;
using SRV6_TipoIdentificacion.DTOs;
using SRV6_TipoIdentificacion.Entities;
using SRV6_TipoIdentificacion.Interfaces;

namespace SRV6_TipoIdentificacion.Services;

public class TipoIdentificacionService : ITipoIdentificacionService
{
    private readonly ApplicationDbContext _db;

    public TipoIdentificacionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TipoIdentificacionDto>> GetAllAsync()
    {
        return await _db.TiposIdentificacion
            .Select(t => new TipoIdentificacionDto { Id = t.Id, Nombre = t.Nombre })
            .ToListAsync();
    }

    public async Task<TipoIdentificacionDto?> GetByIdAsync(int id)
    {
        var tipo = await _db.TiposIdentificacion.FindAsync(id);
        if (tipo == null) return null;

        return new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre };
    }

    public async Task<(bool ok, string error, TipoIdentificacionDto? data)> CreateAsync(CrearTipoIdentificacionDto dto)
    {
        // Validar que el nombre no esté vacío
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return (false, "El nombre es requerido", null);

        // Validar que no exista un tipo con el mismo nombre
        var existe = await _db.TiposIdentificacion.AnyAsync(t => t.Nombre == dto.Nombre);
        if (existe)
            return (false, $"El tipo de identificación '{dto.Nombre}' ya existe", null);

        // Crear el nuevo tipo
        var tipo = new TipoIdentificacion { Nombre = dto.Nombre };
        _db.TiposIdentificacion.Add(tipo);
        await _db.SaveChangesAsync();

        return (true, "Tipo de identificación creado exitosamente", new TipoIdentificacionDto
        {
            Id = tipo.Id,
            Nombre = tipo.Nombre
        });
    }

    public async Task<(bool ok, string error, TipoIdentificacionDto? data)> UpdateAsync(int id, CrearTipoIdentificacionDto dto)
    {
        // Validar que el nombre no esté vacío
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return (false, "El nombre es requerido", null);

        // Buscar el tipo a actualizar
        var tipo = await _db.TiposIdentificacion.FindAsync(id);
        if (tipo == null)
            return (false, $"Tipo de identificación con ID {id} no encontrado", null);

        // Validar que no exista otro tipo con el mismo nombre
        var existe = await _db.TiposIdentificacion.AnyAsync(t => t.Nombre == dto.Nombre && t.Id != id);
        if (existe)
            return (false, $"Ya existe otro tipo con el nombre '{dto.Nombre}'", null);

        var nombreAnterior = tipo.Nombre;
        tipo.Nombre = dto.Nombre;
        await _db.SaveChangesAsync();

        return (true, $"Se cambió de '{nombreAnterior}' a '{tipo.Nombre}'", new TipoIdentificacionDto
        {
            Id = tipo.Id,
            Nombre = tipo.Nombre
        });
    }

    public async Task<(bool ok, string error)> DeleteAsync(int id)
    {
        var tipo = await _db.TiposIdentificacion.FindAsync(id);
        if (tipo == null)
            return (false, $"Tipo de identificación con ID {id} no encontrado");

        _db.TiposIdentificacion.Remove(tipo);
        await _db.SaveChangesAsync();

        return (true, $"Tipo de identificación '{tipo.Nombre}' (ID: {id}) eliminado exitosamente");
    }

    public async Task<bool> ValidarExistenciaAsync(string nombre)
    {
        return await _db.TiposIdentificacion.AnyAsync(t => t.Nombre == nombre);
=======
using TipoIdentificacionSRV6.Data;
using TipoIdentificacionSRV6.DTOs;
using TipoIdentificacionSRV6.Entities;

namespace TipoIdentificacionSRV6.Services
{
    public class TipoIdentificacionService : ITipoIdentificacionService
    {
        private readonly ApplicationDbContext _db;

        public TipoIdentificacionService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TipoIdentificacionDto>> GetAllAsync()
        {
            return await _db.TiposIdentificacion
                .OrderBy(t => t.Nombre)
                .Select(t => new TipoIdentificacionDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre
                })
                .ToListAsync();
        }

        public async Task<TipoIdentificacionDto?> GetByIdAsync(int id)
        {
            var entity = await _db.TiposIdentificacion
                .FirstOrDefaultAsync(t => t.Id == id);  // ✅ CORREGIDO: t.Id == id

            if (entity == null) return null;

            return new TipoIdentificacionDto
            {
                Id = entity.Id,
                Nombre = entity.Nombre
                // ❌ ELIMINADO: Activo, FechaCreacion
            };
        }

        public async Task<int> CreateAsync(TipoIdentificacionCreateDto dto)
        {
            var entity = new TipoIdentificacion
            {
                Nombre = dto.Nombre.Trim()
                // ❌ ELIMINADO: Activo, FechaCreacion
            };

            _db.TiposIdentificacion.Add(entity);
            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> UpdateAsync(TipoIdentificacionUpdateDto dto)
        {
            var entity = await _db.TiposIdentificacion
                .FirstOrDefaultAsync(t => t.Id == dto.Id);  // ✅ CORREGIDO

            if (entity == null) return 0;

            entity.Nombre = dto.Nombre.Trim();
            // ❌ ELIMINADO: FechaModificacion

            await _db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<int> DeleteAsync(int id)
        {
            var entity = await _db.TiposIdentificacion
                .FirstOrDefaultAsync(t => t.Id == id);  // ✅ CORREGIDO

            if (entity == null) return 0;

            _db.TiposIdentificacion.Remove(entity);  // ✅ ELIMINACIÓN FÍSICA
            await _db.SaveChangesAsync();
            return id;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.TiposIdentificacion
                .AnyAsync(t => t.Id == id);  // ✅ CORREGIDO
        }

        public async Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null)
        {
            var query = _db.TiposIdentificacion
                .Where(t => t.Nombre == nombre.Trim());  // ✅ CORREGIDO

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
>>>>>>> a7a79ac (Actualizacion del Login)
    }
}