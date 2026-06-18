using SRV15_Parametro.Entities;

namespace SRV15_Parametro.Services
{
    public interface IParametroService
    {
        Task<IEnumerable<Parametro>> GetAllAsync();
        Task<Parametro?> GetByIdAsync(string id);
        // status: 201 creado | 400 datos faltantes | 422 formato invalido | 409 ya existe
        Task<(Parametro? data, int status)> CreateAsync(ParametroRequest request);
        // status: 200 ok | 400 datos faltantes | 422 formato invalido | 404 no existe
        Task<(Parametro? data, int status)> UpdateAsync(string id, ParametroRequest request);
        // status: 200 ok | 404 no existe
        Task<(Parametro? data, int status)> DeleteAsync(string id);
    }
}
