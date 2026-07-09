using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RolSRV8.Services;

public class BitacoraClient : IBitacoraClient
{
    private readonly HttpClient _httpClient;

    public BitacoraClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task RegistrarAsync(
        string usuario,
        string accion,
        string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        await _httpClient.PostAsJsonAsync(
            "http://localhost:5209/bitacora",
            new
            {
                Usuario = usuario,
                Accion = accion
            });
    }
}