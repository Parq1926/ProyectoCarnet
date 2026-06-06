using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TipoIdentificacionSRV6.Data;
using TipoIdentificacionSRV6.DTOs;
using TipoIdentificacionSRV6.Models;

namespace TipoIdentificacionSRV6.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TipoIdentificacionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TipoIdentificacionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoIdentificacionDto>>> GetAll()
    {
        var tipos = await _context.TiposIdentificacion
            .Select(t => new TipoIdentificacionDto { Id = t.Id, Nombre = t.Nombre })
            .ToListAsync();
        return Ok(tipos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoIdentificacionDto>> GetById(int id)
    {
        var tipo = await _context.TiposIdentificacion.FindAsync(id);
        if (tipo == null)
            return NotFound(new { mensaje = "Tipo de identificación no encontrado" });
        return Ok(new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre });
    }

    [HttpPost]
    public async Task<ActionResult<TipoIdentificacionDto>> Create([FromBody] CrearTipoIdentificacionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest(new { mensaje = "El nombre es requerido" });

        var existe = await _context.TiposIdentificacion.AnyAsync(t => t.Nombre == dto.Nombre);
        if (existe)
            return Conflict(new { mensaje = "El tipo de identificación ya existe" });

        var tipo = new TipoIdentificacion { Nombre = dto.Nombre };
        _context.TiposIdentificacion.Add(tipo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tipo.Id },
            new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CrearTipoIdentificacionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest(new { mensaje = "El nombre es requerido" });

        var tipo = await _context.TiposIdentificacion.FindAsync(id);
        if (tipo == null)
            return NotFound(new { mensaje = "Tipo de identificación no encontrado" });

        var existe = await _context.TiposIdentificacion.AnyAsync(t => t.Nombre == dto.Nombre && t.Id != id);
        if (existe)
            return Conflict(new { mensaje = "Ya existe otro tipo con ese nombre" });

        tipo.Nombre = dto.Nombre;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Tipo de identificación actualizado", tipo = new TipoIdentificacionDto { Id = tipo.Id, Nombre = tipo.Nombre } });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tipo = await _context.TiposIdentificacion.FindAsync(id);
        if (tipo == null)
            return NotFound(new { mensaje = "Tipo de identificación no encontrado" });

        _context.TiposIdentificacion.Remove(tipo);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Tipo de identificación eliminado" });
    }
    [HttpGet("publico")]
    public IActionResult Publico()
    {
        return Ok(new { mensaje = "Este endpoint no requiere autenticación", timestamp = DateTime.Now });
    }
}