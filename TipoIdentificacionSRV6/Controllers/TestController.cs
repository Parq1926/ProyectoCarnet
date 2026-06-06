using Microsoft.AspNetCore.Mvc;

namespace TipoIdentificacionSRV6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { mensaje = "Pong!", timestamp = DateTime.Now });
    }
}