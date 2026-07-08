// Pages/Login.cshtml.cs
using LoginSRV1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LoginSRV1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;

        public LoginModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Usuario { get; set; } = string.Empty;

        [BindProperty]
        public string Contrasena { get; set; } = string.Empty;

        [BindProperty]
        public string Tipo { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? RedirectMessage { get; set; }
        public int LoginAttempts { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Verificar si hay mensaje de redirección
            RedirectMessage = TempData["RedirectMessage"] as string;

            if (await _authService.IsAuthenticatedAsync())
            {
                return RedirectToPage("/Welcome");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Usuario) ||
                string.IsNullOrWhiteSpace(Contrasena) ||
                string.IsNullOrWhiteSpace(Tipo))
            {
                ErrorMessage = "Todos los campos son requeridos";
                return Page();
            }

            LoginAttempts = HttpContext.Session.GetInt32($"LoginAttempts_{Usuario}") ?? 0;

            var response = await _authService.LoginAsync(Usuario, Contrasena, Tipo);

            if (response != null && response.Codigo == 200)
            {
                HttpContext.Session.SetInt32($"LoginAttempts_{Usuario}", 0);
                return RedirectToPage("/Welcome");
            }

            // ✅ Si es código 403, mostrar mensaje de bloqueo
            if (response != null && response.Codigo == 403)
            {
                ErrorMessage = response.Mensaje ?? "Usuario bloqueado permanentemente. Contacte al administrador.";
                return Page();
            }

            LoginAttempts++;
            HttpContext.Session.SetInt32($"LoginAttempts_{Usuario}", LoginAttempts);

            if (LoginAttempts >= 3)
            {
                ErrorMessage = "Ha excedido el número de intentos. Usuario bloqueado.";
                return Page();
            }

            ErrorMessage = response?.Mensaje ?? "Usuario y/o contraseña incorrectos";
            ErrorMessage += $" Intentos restantes: {3 - LoginAttempts}";
            return Page();
        }
    }
}