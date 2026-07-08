using SRV6_TipoIdentificacion.DTOs;

namespace SRV6_TipoIdentificacion.Interfaces;

public interface ITipoIdentificacionService
{
    Task<IEnumerable<TipoIdentificacionDto>> GetAllAsync();
    Task<TipoIdentificacionDto?> GetByIdAsync(int id);
    Task<(bool ok, string error, TipoIdentificacionDto? data)> CreateAsync(CrearTipoIdentificacionDto dto);
    Task<(bool ok, string error, TipoIdentificacionDto? data)> UpdateAsync(int id, CrearTipoIdentificacionDto dto);
    Task<(bool ok, string error)> DeleteAsync(int id);
    Task<bool> ValidarExistenciaAsync(string nombre);
}