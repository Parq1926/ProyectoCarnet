using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TipoUsuarioSRV5.Data;
using TipoUsuarioSRV5.DTOs;
using TipoUsuarioSRV5.Models;

namespace TipoUsuarioSRV5.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TipoUsuarioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TipoUsuarioController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TipoUsuarioDto>>> GetAll()
    {
        var tipos = await _context.TiposUsuario
            .Select(t => new TipoUsuarioDto { Id = t.Id, Nombre = t.Nombre })
            .ToListAsync();
        return Ok(tipos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TipoUsuarioDto>> GetById(int id)
    {
        var tipo = await _context.TiposUsuario.FindAsync(id);
        if (tipo == null)
            return NotFound(new { mensaje = "Tipo de usuario no encontrado" });
        return Ok(new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre });
    }

    [HttpPost]
    public async Task<ActionResult<TipoUsuarioDto>> Create([FromBody] CrearTipoUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest(new { mensaje = "El nombre es requerido" });

        var existe = await _context.TiposUsuario.AnyAsync(t => t.Nombre == dto.Nombre);
        if (existe)
            return Conflict(new { mensaje = "El tipo de usuario ya existe" });

        var tipo = new TipoUsuario { Nombre = dto.Nombre };
        _context.TiposUsuario.Add(tipo);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = tipo.Id },
            new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CrearTipoUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            return BadRequest(new { mensaje = "El nombre es requerido" });

        var tipo = await _context.TiposUsuario.FindAsync(id);
        if (tipo == null)
            return NotFound(new { mensaje = "Tipo de usuario no encontrado" });

        var existe = await _context.TiposUsuario.AnyAsync(t => t.Nombre == dto.Nombre && t.Id != id);
        if (existe)
            return Conflict(new { mensaje = "Ya existe otro tipo con ese nombre" });

        tipo.Nombre = dto.Nombre;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Tipo de usuario actualizado", tipo = new TipoUsuarioDto { Id = tipo.Id, Nombre = tipo.Nombre } });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tipo = await _context.TiposUsuario.FindAsync(id);
        if (tipo == null)
            return NotFound(new { mensaje = "Tipo de usuario no encontrado" });

        _context.TiposUsuario.Remove(tipo);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Tipo de usuario eliminado" });
    }

    [HttpGet("validar/{nombre}")]
    public async Task<IActionResult> ValidarTipo(string nombre)
    {
        var existe = await _context.TiposUsuario.AnyAsync(t => t.Nombre == nombre);
        if (existe)
            return Ok(new { existe = true, mensaje = "Tipo válido" });
        else
            return NotFound(new { existe = false, mensaje = "Tipo no encontrado" });
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { mensaje = "Pong!", timestamp = DateTime.Now });
    }
}