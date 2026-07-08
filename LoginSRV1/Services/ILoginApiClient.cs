// Services/ILoginApiClient.cs
using LoginSRV1.DTOs;

namespace LoginSRV1.Services
{
    public interface ILoginApiClient
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto login, CancellationToken ct = default);
        Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default);
        Task<RefreshResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    }
}