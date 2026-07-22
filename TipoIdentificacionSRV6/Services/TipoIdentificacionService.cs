using Microsoft.EntityFrameworkCore;
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
    }
}