using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TipoIdentificacionSRV6.Pages.TiposIdentificacion
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
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Redirect("https://localhost:7019/Login");
                }
            }
            return Page();
        }
    }
}