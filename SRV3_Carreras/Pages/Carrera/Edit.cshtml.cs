using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRV3_Carreras.Services;

namespace SRV3_Carreras.Pages.Carrera
{
    public class EditModel : PageModel
    {
        private readonly ICarreraService _carreraService;

        public EditModel(ICarreraService carreraService)
        {
            _carreraService = carreraService;
        }

        [BindProperty]
        public UpdateCarreraRequest Request { get; set; } = new UpdateCarreraRequest();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var carrera = await _carreraService.GetById(id);

            if (carrera == null)
            {
                return NotFound();
            }

            Request.ID = carrera.ID;
            Request.Nombre = carrera.Nombre;
            Request.Director = carrera.Director;
            Request.Email = carrera.Email;
            Request.Telefono = carrera.Telefono;
            Request.InstitucionID = carrera.InstitucionID;
            Request.InstitucionNombre = carrera.InstitucionNombre;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _carreraService.Update(Request);

            if (result.success)
            {
                TempData["Success"] = "Carrera actualizada exitosamente";
                return RedirectToPage("/Carrera/Index");
            }

            ModelState.AddModelError(string.Empty, result.message);
            return Page();
        }
    }
}