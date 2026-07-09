using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SRV11_AutoRegistro.Services;

public interface IInstitucionService
{
    Task<InstitucionDto?> GetById(int id);
}
public class InstitucionService : IInstitucionService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public InstitucionService(
        HttpClient httpClient,
        IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<InstitucionDto?> GetById(int id)
    {
        var token = await _authService.ObtenerTokenAsync();

        if (string.IsNullOrEmpty(token))
            return null;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return await _httpClient.GetFromJsonAsync<InstitucionDto>(
            $"http://localhost:7002/institucion/{id}");
    }
}

public class InstitucionDto
{
    public int ID { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Dominios { get; set; } = string.Empty;
}