using Microsoft.AspNetCore.Mvc.RazorPages;
using SRV3_Carreras.Entities;
using SRV3_Carreras.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRV3_Carreras.Pages.Carrera
{
    public class IndexModel : PageModel
    {
        private readonly ICarreraService _carreraService;

        public IndexModel(ICarreraService carreraService)
        {
            _carreraService = carreraService;
        }

        public IEnumerable<SRV3_Carreras.Entities.Carrera> Carreras { get; set; } = new List<SRV3_Carreras.Entities.Carrera>();

        public async Task OnGetAsync()
        {
            Carreras = await _carreraService.GetAll();
        }
    }
}