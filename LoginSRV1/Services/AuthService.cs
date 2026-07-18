<<<<<<< HEAD
﻿// Services/AuthService.cs
using LoginSRV1.DTOs;
=======
﻿using LoginSRV1.DTOs;
>>>>>>> a7a79ac (Actualizacion del Login)
using Microsoft.AspNetCore.Http;

namespace LoginSRV1.Services
{
    public class AuthService : IAuthService
    {
<<<<<<< HEAD
        private readonly ILoginApiClient _loginApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            ILoginApiClient loginApiClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _loginApiClient = loginApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session!;

        
        public async Task<LoginResponseDto?> LoginAsync(string usuario, string contrasena, string tipo)
        {
            var loginDto = new LoginDto
            {
                Usuario = usuario,
                Contrasena = contrasena,
                Tipo = tipo
            };

            var response = await _loginApiClient.LoginAsync(loginDto);

            if (response != null && response.Codigo == 200)
            {
                Session.SetString("AccessToken", response.AccessToken);
                Session.SetString("RefreshToken", response.RefreshToken);
                Session.SetInt32("UsuarioId", response.UsuarioId);
                Session.SetString("UsuarioEmail", usuario);
                Session.SetString("UsuarioTipo", tipo);
                Session.SetString("NombreCompleto", usuario);
            }

            return response;
=======
        private readonly IUsuarioApiClient _usuarioApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;

        public AuthService(
            IUsuarioApiClient usuarioApiClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _usuarioApiClient = usuarioApiClient;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext?.Session!;
        }

        public async Task<LoginResponseDto?> LoginAsync(string usuario, string contrasena, string tipo)
        {
            var response = await _usuarioApiClient.ValidarCredencialesAsync(usuario, contrasena, tipo);

            if (response != null)
            {
                _session.SetString("AccessToken", "token-temp");
                _session.SetInt32("UsuarioId", response.Id);
                _session.SetString("UsuarioEmail", usuario);
                _session.SetString("UsuarioTipo", tipo);
                _session.SetString("NombreCompleto", response.NombreCompleto);

                return new LoginResponseDto
                {
                    Codigo = 200,
                    Mensaje = "Login exitoso",
                    UsuarioId = response.Id,
                    Institutions = new[] { "CUC" }
                };
            }

            return new LoginResponseDto
            {
                Codigo = 401,
                Mensaje = "Usuario y/o contraseña incorrectos"
            };
>>>>>>> a7a79ac (Actualizacion del Login)
        }

        public Task<bool> LogoutAsync()
        {
<<<<<<< HEAD
            Session.Clear();
=======
            _session.Clear();
>>>>>>> a7a79ac (Actualizacion del Login)
            return Task.FromResult(true);
        }

        public Task<bool> IsAuthenticatedAsync()
        {
<<<<<<< HEAD
            var token = Session.GetString("AccessToken");
=======
            var token = _session.GetString("AccessToken");
>>>>>>> a7a79ac (Actualizacion del Login)
            return Task.FromResult(!string.IsNullOrEmpty(token));
        }

        public string? GetAccessToken()
        {
<<<<<<< HEAD
            return Session.GetString("AccessToken");
=======
            return _session.GetString("AccessToken");
>>>>>>> a7a79ac (Actualizacion del Login)
        }

        public string? GetRefreshToken()
        {
<<<<<<< HEAD
            return Session.GetString("RefreshToken");
=======
            return _session.GetString("RefreshToken");
>>>>>>> a7a79ac (Actualizacion del Login)
        }

        public int? GetUsuarioId()
        {
<<<<<<< HEAD
            return Session.GetInt32("UsuarioId");
=======
            return _session.GetInt32("UsuarioId");
>>>>>>> a7a79ac (Actualizacion del Login)
        }
    }
}