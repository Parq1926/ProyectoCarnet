using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TiposUsuarioSRV5.Pages.TiposUsuario
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Verificar si el token está presente
            var token = Request.Query["token"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                // Intentar obtener de la cookie o header
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    // Redirigir a Login
                    return Redirect("https://localhost:7019/Login");
                }
            }
            return Page();
        }
    }
}