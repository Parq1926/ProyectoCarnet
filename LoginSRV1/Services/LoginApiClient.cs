// Services/LoginApiClient.cs
using LoginSRV1.DTOs;
using System.Net.Http.Json;

namespace LoginSRV1.Services
{
    public class LoginApiClient : ILoginApiClient
    {
        private readonly HttpClient _http;

        public LoginApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto login, CancellationToken ct = default)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/Auth/login", login, ct);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>(ct);
                }

                // Intentar leer mensaje de error
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(ct);
                if (error != null && error.TryGetValue("mensaje", out var mensaje))
                {
                    return new LoginResponseDto
                    {
                        Codigo = (int)response.StatusCode,
                        Mensaje = mensaje?.ToString() ?? "Error en el login"
                    };
                }

                return new LoginResponseDto
                {
                    Codigo = (int)response.StatusCode,
                    Mensaje = "Error al iniciar sesión"
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    Codigo = 500,
                    Mensaje = $"Error de conexión: {ex.Message}"
                };
            }
        }

        public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "api/Auth/validate");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _http.SendAsync(request, ct);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RefreshResponseDto?> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            try
            {
                var request = new RefreshTokenRequestDto { RefreshToken = refreshToken };
                var response = await _http.PostAsJsonAsync("api/Auth/refresh", request, ct);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RefreshResponseDto>(ct);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}