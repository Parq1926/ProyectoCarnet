using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SRV11_AutoRegistro.Services;

public interface ICarreraService
{
    Task<CarreraDto?> GetById(int id);
}

public class CarreraService : ICarreraService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    public CarreraService(
        HttpClient httpClient,
        IConfiguration configuration,
        IAuthService authService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _authService = authService;
    }


    public async Task<CarreraDto?> GetById(int id)
    {
        try
        {
            var carreraUrl = _configuration["Services:Carrera"];

            var token = await _authService.ObtenerTokenAsync();

            if (string.IsNullOrEmpty(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await _httpClient.GetFromJsonAsync<CarreraDto>(
                $"{carreraUrl}/carreras/{id}");
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