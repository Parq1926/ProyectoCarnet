<<<<<<< HEAD
﻿// Services/IAuthService.cs
using LoginSRV1.DTOs;
=======
﻿using LoginSRV1.DTOs;
>>>>>>> a7a79ac (Actualizacion del Login)

namespace LoginSRV1.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(string usuario, string contrasena, string tipo);
        Task<bool> LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        string? GetAccessToken();
        string? GetRefreshToken();
        int? GetUsuarioId();
    }
}