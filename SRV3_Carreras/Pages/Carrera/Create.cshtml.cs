using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRV3_Carreras.Services;

namespace SRV3_Carreras.Pages.Carrera
{
    public class CreateModel : PageModel
    {
        private readonly ICarreraService _carreraService;

        public CreateModel(ICarreraService carreraService)
        {
            _carreraService = carreraService;
        }

        [BindProperty]
        public CreateCarreraRequest Request { get; set; } = new CreateCarreraRequest();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _carreraService.Create(Request);

            if (result.success)
            {
                TempData["Success"] = "Carrera creada exitosamente";
                return RedirectToPage("/Carrera/Index");
            }

            ModelState.AddModelError(string.Empty, result.message);
            return Page();
        }
    }
}