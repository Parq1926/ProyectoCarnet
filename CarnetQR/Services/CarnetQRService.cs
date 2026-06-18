using QRCoder;
using SRV14_CarnetQR.Entities;
using System.Text.Json;
using System.Text.Encodings.Web;

// Nota: CarnetQRData se usa internamente para armar el contenido del QR.
// Este microservicio NO usa base de datos. Los datos del carnet se simulan
// en memoria (variables) porque la información cambia constantemente y la BD
// aún no existe. Por la misma razón, el QR generado tampoco se persiste.

namespace SRV14_CarnetQR.Services
{
    public class CarnetQRService : ICarnetQRService
    {
        // ===== Institución (simulada como variable, no viene de base de datos) =====
        // institucion = CUC
        private const string INSTITUCION = "CUC";

        // Fecha de vencimiento por defecto del carnet (simulada).
        private static readonly DateTime FECHA_VENCIMIENTO = new DateTime(2026, 12, 31);

        public Task<string?> GenerarQRAsync(string identificacion)
        {
            var datosCarnet = ObtenerDatosSimulados(identificacion);
            if (datosCarnet is null)
                return Task.FromResult<string?>(null);

            // Se arma el JSON del carnet con la misma estructura solicitada en la HU SRV14.
            var jsonContenido = JsonSerializer.Serialize(datosCarnet, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                // Evita que los acentos se escapen como \u00ED, etc.
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            var qrBase64 = GenerarQRBase64(jsonContenido);

            // No se guarda en base de datos: el resultado se devuelve directamente.
            return Task.FromResult<string?>(qrBase64);
        }

        private static string GenerarQRBase64(string contenido)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var pngBytes = qrCode.GetGraphic(10);
            return Convert.ToBase64String(pngBytes);
        }

        // ===== Datos simulados en memoria (reemplazan a la base de datos) =====
        // La institución se asigna desde la variable INSTITUCION = "CUC".
        private static CarnetQRData? ObtenerDatosSimulados(string identificacion)
        {
            var usuarios = new Dictionary<string, CarnetQRData>(StringComparer.OrdinalIgnoreCase)
            {
                ["1-0234-5678"] = new CarnetQRData
                {
                    NombreCompleto   = "Ana María Solís Herrera",
                    Identificacion   = "1-0234-5678",
                    TipoUsuario      = "Estudiante",
                    CarrerasOAreas   = new List<string> { "Ingeniería en Tecnologías de Información" },
                    Institucion      = INSTITUCION,
                    FechaVencimiento = FECHA_VENCIMIENTO
                },
                ["2-0456-7890"] = new CarnetQRData
                {
                    NombreCompleto   = "Carlos Eduardo Mora Jiménez",
                    Identificacion   = "2-0456-7890",
                    TipoUsuario      = "Funcionario",
                    CarrerasOAreas   = new List<string> { "Departamento de Registro", "Departamento de TI" },
                    Institucion      = INSTITUCION,
                    FechaVencimiento = FECHA_VENCIMIENTO
                },
                ["3-0678-9012"] = new CarnetQRData
                {
                    NombreCompleto   = "Laura Beatriz Vargas Campos",
                    Identificacion   = "3-0678-9012",
                    TipoUsuario      = "Estudiante",
                    CarrerasOAreas   = new List<string> { "Administración de Empresas", "Contabilidad" },
                    Institucion      = INSTITUCION,
                    FechaVencimiento = FECHA_VENCIMIENTO
                },
                ["4-0891-2345"] = new CarnetQRData
                {
                    NombreCompleto   = "Roberto Alvarado Navarro",
                    Identificacion   = "4-0891-2345",
                    TipoUsuario      = "Administrador",
                    CarrerasOAreas   = new List<string> { "Dirección General" },
                    Institucion      = INSTITUCION,
                    FechaVencimiento = FECHA_VENCIMIENTO
                }
            };

            return usuarios.TryGetValue(identificacion, out var datos) ? datos : null;
        }
    }
}
