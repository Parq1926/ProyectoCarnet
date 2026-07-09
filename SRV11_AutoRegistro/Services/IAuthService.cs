using System.Text.Json;

namespace SRV11_AutoRegistro.Services;

public interface IAuthService
{
    Task<string?> ObtenerTokenAsync();
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string?> ObtenerTokenAsync()
    {
        var loginUrl =
            $"{_configuration["Services:Login"]}/api/Auth/login";

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            loginUrl);

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