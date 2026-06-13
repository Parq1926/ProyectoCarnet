using System;

namespace SRV11_AutoRegistro.Entities;

public class Usuario
{
    public int ID { get; set; }

    public string Email { get; set; } = string.Empty;

    public int TipoIdentificacionID { get; set; }

    public string Identificacion { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public int TipoUsuarioID { get; set; }

    public int RolID { get; set; }

    public bool Confirmado { get; set; }

    public string? TokenConfirmacion { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public List<int> Instituciones { get; set; } = new();

    public List<int> Carreras { get; set; } = new();

    public List<int> Areas { get; set; } = new();

    public List<string> Telefonos { get; set; } = new();

}