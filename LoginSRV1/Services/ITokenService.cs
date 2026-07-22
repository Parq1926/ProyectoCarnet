// Services/ITokenService.cs
using LoginSRV1.Entities;

namespace LoginSRV1.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(Usuario user);
        string GenerateRefreshToken();
    }
}