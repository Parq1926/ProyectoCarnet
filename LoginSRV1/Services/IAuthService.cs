// Services/IAuthService.cs
using LoginSRV1.DTOs;

namespace LoginSRV1.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(string usuario, string contrasena, string tipo);
        Task<bool> LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        string? GetAccessToken();
        string? GetRefreshToken();
        int? GetUsuarioId();
    }
}