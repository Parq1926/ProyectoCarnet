using Microsoft.EntityFrameworkCore;
using SRV5_TipoUsuario.Data;
using SRV5_TipoUsuario.DTOs;
using SRV5_TipoUsuario.Entities;
using SRV5_TipoUsuario.Interfaces;

namespace SRV5_TipoUsuario.Services;

public class TipoUsuarioService : ITipoUsuarioService
{
    private readonly ApplicationDbContext _db;

    public TipoUsuarioService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TipoUsuarioDto>> GetAllAsync()
    {
        return await _db.TiposUsuario
            .Select(t => new TipoUsuarioDto { Id = t.Id, Nombre = t.Nombre })
            .ToListAsync();
    }

    public async Task<TipoUsuarioDto?> GetByIdAsync(int id)
    {
        var tipo = await _db.TiposUsuario.FindAsync(id);
        if (tipo == null) return null;

        return new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre };
    }

    public async Task<(bool ok, string error, TipoUsuarioDto? data)> CreateAsync(CrearTipoUsuarioDto dto)
    {
        // Validar que el nombre no esté vacío
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return (false, "El nombre es requerido", null);

        // Validar que no exista un tipo con el mismo nombre
        var existe = await _db.TiposUsuario.AnyAsync(t => t.Nombre == dto.Nombre);
        if (existe)
            return (false, $"El tipo de usuario '{dto.Nombre}' ya existe", null);

        // Crear el nuevo tipo
        var tipo = new TipoUsuario { Nombre = dto.Nombre };
        _db.TiposUsuario.Add(tipo);
        await _db.SaveChangesAsync();

        return (true, "Tipo de usuario creado exitosamente", new TipoUsuarioDto
        {
            Id = tipo.Id,
            Nombre = tipo.Nombre
        });
    }

    public async Task<(bool ok, string error, TipoUsuarioDto? data)> UpdateAsync(int id, CrearTipoUsuarioDto dto)
    {
        // Validar que el nombre no esté vacío
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return (false, "El nombre es requerido", null);

        // Buscar el tipo a actualizar
        var tipo = await _db.TiposUsuario.FindAsync(id);
        if (tipo == null)
            return (false, $"Tipo de usuario con ID {id} no encontrado", null);

        // Validar que no exista otro tipo con el mismo nombre
        var existe = await _db.TiposUsuario.AnyAsync(t => t.Nombre == dto.Nombre && t.Id != id);
        if (existe)
            return (false, $"Ya existe otro tipo con el nombre '{dto.Nombre}'", null);

        var nombreAnterior = tipo.Nombre;
        tipo.Nombre = dto.Nombre;
        await _db.SaveChangesAsync();

        return (true, $"Se cambió de '{nombreAnterior}' a '{tipo.Nombre}'", new TipoUsuarioDto
        {
            Id = tipo.Id,
            Nombre = tipo.Nombre
        });
    }

    public async Task<(bool ok, string error)> DeleteAsync(int id)
    {
        var tipo = await _db.TiposUsuario.FindAsync(id);
        if (tipo == null)
            return (false, $"Tipo de usuario con ID {id} no encontrado");

        _db.TiposUsuario.Remove(tipo);
        await _db.SaveChangesAsync();

        return (true, $"Tipo de usuario '{tipo.Nombre}' (ID: {id}) eliminado exitosamente");
    }

    public async Task<bool> ValidarExistenciaAsync(string nombre)
    {
        return await _db.TiposUsuario.AnyAsync(t => t.Nombre == nombre);
    }
}