// Services/AuthService.cs
using LoginSRV1.DTOs;
using Microsoft.AspNetCore.Http;

namespace LoginSRV1.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILoginApiClient _loginApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            ILoginApiClient loginApiClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _loginApiClient = loginApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session!;

        
        public async Task<LoginResponseDto?> LoginAsync(string usuario, string contrasena, string tipo)
        {
            var loginDto = new LoginDto
            {
                Usuario = usuario,
                Contrasena = contrasena,
                Tipo = tipo
            };

            var response = await _loginApiClient.LoginAsync(loginDto);

            if (response != null && response.Codigo == 200)
            {
                Session.SetString("AccessToken", response.AccessToken);
                Session.SetString("RefreshToken", response.RefreshToken);
                Session.SetInt32("UsuarioId", response.UsuarioId);
                Session.SetString("UsuarioEmail", usuario);
                Session.SetString("UsuarioTipo", tipo);
                Session.SetString("NombreCompleto", usuario);
            }

            return response;
        }

        public Task<bool> LogoutAsync()
        {
            Session.Clear();
            return Task.FromResult(true);
        }

        public Task<bool> IsAuthenticatedAsync()
        {
            var token = Session.GetString("AccessToken");
            return Task.FromResult(!string.IsNullOrEmpty(token));
        }

        public string? GetAccessToken()
        {
            return Session.GetString("AccessToken");
        }

        public string? GetRefreshToken()
        {
            return Session.GetString("RefreshToken");
        }

        public int? GetUsuarioId()
        {
            return Session.GetInt32("UsuarioId");
        }
    }
}