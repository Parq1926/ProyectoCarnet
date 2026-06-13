using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SRV1_Login.Entities;

namespace SRV1_Login.Services;

public interface ITokenService
{
    string GenerateJwtToken(Usuario user);
    string GenerateRefreshToken();
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateJwtToken(Usuario user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "MiClavePredeterminada1234567890!!");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.NombreCompleto),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.TipoUsuario?.Nombre ?? "Usuario")
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