using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SRV3_Carreras.Entities;
using SRV3_Carreras.Services;

namespace SRV3_Carreras.Pages.Carrera
{
    public class DetailsModel : PageModel
    {
        private readonly ICarreraService _carreraService;

        public DetailsModel(ICarreraService carreraService)
        {
            _carreraService = carreraService;
        }

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
    }
}