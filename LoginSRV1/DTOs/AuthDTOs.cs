<<<<<<< HEAD
﻿// DTOs/AuthDTOs.cs
namespace LoginSRV1.DTOs
=======
﻿namespace LoginSRV1.DTOs
>>>>>>> a7a79ac (Actualizacion del Login)
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

<<<<<<< HEAD
=======
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

>>>>>>> a7a79ac (Actualizacion del Login)
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

<<<<<<< HEAD
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
=======
    // DTOs para comunicación con UsuariosSRV4
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
    }

    public class ValidarCredencialesRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class ValidarCredencialesResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public bool Bloqueado { get; set; }
>>>>>>> a7a79ac (Actualizacion del Login)
    }
}