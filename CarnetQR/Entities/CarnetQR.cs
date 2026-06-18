namespace SRV14_CarnetQR.Entities
{
    // Estructura de datos que se serializa a JSON y se codifica dentro del QR.
    // Coincide con los campos solicitados en la HU SRV14.
    // No se persiste en base de datos; se construye desde datos simulados en memoria.
    public class CarnetQRData
    {
        public string NombreCompleto { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
        public string TipoUsuario { get; set; } = null!;
        public List<string> CarrerasOAreas { get; set; } = new();
        public string Institucion { get; set; } = null!;
        public DateTime FechaVencimiento { get; set; }
    }
}
