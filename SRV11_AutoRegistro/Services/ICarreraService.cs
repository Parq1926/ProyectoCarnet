using System.Net.Http.Json;

namespace SRV11_AutoRegistro.Services;

public interface ICarreraService
{
    Task<CarreraDto?> GetById(int id);
}

public class CarreraService : ICarreraService
{
    private readonly HttpClient _httpClient;

    public CarreraService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CarreraDto?> GetById(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<CarreraDto>(
                $"http://localhost:7003/carreras/{id}");
        }
        catch
        {
            return null;
        }
    }
}

public class CarreraDto
{
    public int ID { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public int InstitucionID { get; set; }
}