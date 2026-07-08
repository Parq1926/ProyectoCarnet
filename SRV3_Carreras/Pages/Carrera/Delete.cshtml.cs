using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRV3_Carreras.Entities;
using SRV3_Carreras.Services;

namespace SRV3_Carreras.Pages.Carrera
{
    public class DeleteModel : PageModel
    {
        private readonly ICarreraService _carreraService;

        public DeleteModel(ICarreraService carreraService)
        {
            _carreraService = carreraService;
        }

        [BindProperty]
        public SRV3_Carreras.Entities.Carrera Carrera { get; set; } = new SRV3_Carreras.Entities.Carrera();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var carrera = await _carreraService.GetById(id);

            if (carrera == null)
            {
                return NotFound();
            }

            Carrera = carrera;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _carreraService.Delete(Carrera.ID);

            if (result.success)
            {
                TempData["Success"] = "Carrera eliminada exitosamente";
            }
            else
            {
                TempData["Error"] = result.message;
            }

            return RedirectToPage("/Carrera/Index");
        }
    }
}