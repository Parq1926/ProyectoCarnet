using System.Net.Http.Json;

namespace SRV11_AutoRegistro.Services;

public interface IAreaService
{
    Task<AreaDto?> GetById(int id);
}

public class AreaService : IAreaService
{
    private readonly HttpClient _httpClient;

    public AreaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AreaDto?> GetById(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AreaDto>(
                $"http://localhost:5202/areas/{id}");
        }
        catch
        {
            return null;
        }
    }
}

public class AreaDto
{
    public int ID { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int InstitucionID { get; set; }
}