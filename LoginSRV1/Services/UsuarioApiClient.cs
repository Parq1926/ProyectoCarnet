using LoginSRV1.DTOs;
using System.Net.Http.Json;

namespace LoginSRV1.Services
{
    public interface IUsuarioApiClient
    {
        Task<ValidarCredencialesResponse?> ValidarCredencialesAsync(string email, string password, string tipo, CancellationToken ct = default);
        Task<UsuarioDto?> GetUsuarioByIdAsync(int id, CancellationToken ct = default);
    }

    public class UsuarioApiClient : IUsuarioApiClient
    {
        private readonly HttpClient _http;

        public UsuarioApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ValidarCredencialesResponse?> ValidarCredencialesAsync(string email, string password, string tipo, CancellationToken ct = default)
        {
            try
            {
                var request = new ValidarCredencialesRequest
                {
                    Email = email,
                    Password = password,
                    Tipo = tipo
                };

                var response = await _http.PostAsJsonAsync("api/Usuarios/validar-credenciales", request, ct);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ValidarCredencialesResponse>(ct);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UsuarioDto?> GetUsuarioByIdAsync(int id, CancellationToken ct = default)
        {
            try
            {
                var response = await _http.GetAsync($"api/Usuarios/{id}", ct);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UsuarioDto>(ct);
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