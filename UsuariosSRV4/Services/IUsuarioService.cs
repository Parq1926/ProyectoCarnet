// UsuariosSRV4/Services/IUsuarioService.cs
using UsuariosSRV4.DTOs;

namespace UsuariosSRV4.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<IEnumerable<UsuarioDto>> GetByFilterAsync(FiltroUsuarioDto filtro);
        Task<(bool ok, string error, UsuarioDto? data)> CreateAsync(CrearUsuarioDto dto);
        Task<(bool ok, string error, UsuarioDto? data)> UpdateAsync(int id, ActualizarUsuarioDto dto);
        Task<(bool ok, string error)> DeleteAsync(int id);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
        Task<bool> ExistsByIdentificacionAsync(string identificacion, int? excludeId = null);
        Task<bool> ValidarDominioYTipoAsync(string email, int tipoUsuarioId);
        Task<UsuarioDto?> ValidarCredencialesAsync(string email, string password, string tipo);
        Task<bool> DesbloquearUsuarioAsync(int id);
    }
}