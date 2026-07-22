// DTOs/AuthDTOs.cs
namespace LoginSRV1.DTOs
{
    public class LoginDto
    {
        public string Usuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string[] Institutions { get; set; } = Array.Empty<string>();
    }

    public class RefreshResponseDto
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class TokenValidationDto
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool Valido { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}