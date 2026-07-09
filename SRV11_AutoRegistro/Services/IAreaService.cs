using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SRV11_AutoRegistro.Services;

public interface IAreaService
{
    Task<AreaDto?> GetById(int id);
}

public class AreaService : IAreaService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;

    public AreaService(
        HttpClient httpClient,
        IConfiguration configuration,
        IAuthService authService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _authService = authService;
    }

    public async Task<AreaDto?> GetById(int id)
    {
        try
        {
            var token = await _authService.ObtenerTokenAsync();

            if (string.IsNullOrEmpty(token))
                return null;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var areaUrl = _configuration["Services:Area"];

            return await _httpClient.GetFromJsonAsync<AreaDto>(
                $"{areaUrl}/areas/{id}");
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