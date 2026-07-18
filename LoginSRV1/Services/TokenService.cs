<<<<<<< HEAD
﻿// Services/TokenService.cs
using LoginSRV1.Entities;
=======
﻿using LoginSRV1.DTOs;
>>>>>>> a7a79ac (Actualizacion del Login)
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LoginSRV1.Services
{
<<<<<<< HEAD
=======
    public interface ITokenService
    {
        string GenerateJwtToken(UsuarioDto user);
        string GenerateRefreshToken();
    }

>>>>>>> a7a79ac (Actualizacion del Login)
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

<<<<<<< HEAD
        public string GenerateJwtToken(Usuario user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");

            // ✅ CORREGIDO - Mapeo correcto según tu base de datos
            string roleName = user.TipoUsuarioId switch
            {
                1 => "Administrador",  // ← CORREGIDO
                2 => "Funcionario",    // ← CORREGIDO
                3 => "Estudiante",     // ← CORREGIDO
                _ => "Usuario"
            };

=======
        public string GenerateJwtToken(UsuarioDto user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");

>>>>>>> a7a79ac (Actualizacion del Login)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.NombreCompleto),
                new Claim(ClaimTypes.Email, user.Email),
<<<<<<< HEAD
                new Claim(ClaimTypes.Role, roleName)
=======
                new Claim(ClaimTypes.Role, user.TipoUsuario)
>>>>>>> a7a79ac (Actualizacion del Login)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}