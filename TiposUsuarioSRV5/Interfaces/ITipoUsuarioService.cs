using SRV5_TipoUsuario.DTOs;

namespace SRV5_TipoUsuario.Interfaces;

public interface ITipoUsuarioService
{
    Task<IEnumerable<TipoUsuarioDto>> GetAllAsync();
    Task<TipoUsuarioDto?> GetByIdAsync(int id);
    Task<(bool ok, string error, TipoUsuarioDto? data)> CreateAsync(CrearTipoUsuarioDto dto);
    Task<(bool ok, string error, TipoUsuarioDto? data)> UpdateAsync(int id, CrearTipoUsuarioDto dto);
    Task<(bool ok, string error)> DeleteAsync(int id);
    Task<bool> ValidarExistenciaAsync(string nombre);
}