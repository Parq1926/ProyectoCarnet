using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SRV11_AutoRegistro.Services;

public interface ITipoIdentificacionService
{
    Task<TipoIdentificacionDto?> GetById(int id);
}

public class TipoIdentificacionService : ITipoIdentificacionService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public TipoIdentificacionService(
        HttpClient httpClient,
        IAuthService authService,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _authService = authService;
        _configuration = configuration;
    }

    public async Task<TipoIdentificacionDto?> GetById(int id)
    {
        try
        {
            var token = await _authService.ObtenerTokenAsync();

            Console.WriteLine($"TOKEN: {token}");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);

            var tipoIdentificacionUrl =
                _configuration["Services:TipoIdentificacion"];

            var response =
                await _httpClient.GetAsync(
                    $"{tipoIdentificacionUrl}/api/TipoIdentificacion/{id}");

            Console.WriteLine($"STATUS: {response.StatusCode}");

            var contenido =
                await response.Content.ReadAsStringAsync();

            Console.WriteLine($"CONTENIDO: {contenido}");

            if (!response.IsSuccessStatusCode)
                return null;

            using var document =
                JsonDocument.Parse(contenido);

            var data =
                document.RootElement.GetProperty("data");

            return new TipoIdentificacionDto
            {
                ID = data.GetProperty("id").GetInt32(),
                Nombre = data.GetProperty("nombre").GetString() ?? ""
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }
}

public class TipoIdentificacionDto
{
    public int ID { get; set; }

    public string Nombre { get; set; } = string.Empty;
}