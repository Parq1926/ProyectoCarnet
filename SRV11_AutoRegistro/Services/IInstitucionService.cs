using System.Net.Http.Json;

namespace SRV11_AutoRegistro.Services;

public interface IInstitucionService
{
    Task<InstitucionDto?> GetById(int id);
}

public class InstitucionService : IInstitucionService
{
    private readonly HttpClient _httpClient;

    public InstitucionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<InstitucionDto?> GetById(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<InstitucionDto>(
                $"http://localhost:7002/institucion/{id}");
        }
        catch
        {
            return null;
        }
    }
}

public class InstitucionDto
{
    public int ID { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Dominios { get; set; } = string.Empty;
}