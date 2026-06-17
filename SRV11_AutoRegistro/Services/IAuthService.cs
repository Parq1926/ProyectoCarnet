using System.Text.Json;

namespace SRV11_AutoRegistro.Services;

public interface IAuthService
{
    Task<string?> ObtenerTokenAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> ObtenerTokenAsync()
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "http://localhost:5129/api/Auth/login");

        request.Headers.Add(
            "usuario",
            "servicio@autoregistro.cr");

        request.Headers.Add(
            "contrasena",
            "123456");

        request.Headers.Add(
            "tipo",
            "Estudiante");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        return document.RootElement
            .GetProperty("access_token")
            .GetString();
    }
}