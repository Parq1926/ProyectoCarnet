using SRV15_Parametro.Entities;
using SRV15_Parametro.Repository;
using System.Text.RegularExpressions;

namespace SRV15_Parametro.Services
{
    public class ParametroService : IParametroService
    {
        private readonly ParametroRepository _repository;

        public ParametroService(ParametroRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Parametro>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<Parametro?> GetByIdAsync(string id) =>
            await _repository.GetByIdAsync(id);

        public async Task<(Parametro? data, int status)> CreateAsync(ParametroRequest request)
        {
            var v = Validar(request);
            if (v != 0) return (null, v);

            if (await _repository.GetByIdAsync(request.Id) is not null)
                return (null, 409); // ya existe

            var parametro = new Parametro { Id = request.Id, Valor = request.Valor };
            await _repository.CreateAsync(parametro);
            return (parametro, 201);
        }

        public async Task<(Parametro? data, int status)> UpdateAsync(string id, ParametroRequest request)
        {
            var v = Validar(request);
            if (v != 0) return (null, v);

            if (await _repository.GetByIdAsync(id) is null)
                return (null, 404);

            var parametro = new Parametro { Id = id, Valor = request.Valor };
            await _repository.UpdateAsync(parametro);
            return (parametro, 200);
        }

        public async Task<(Parametro? data, int status)> DeleteAsync(string id)
        {
            var existe = await _repository.GetByIdAsync(id);
            if (existe is null)
                return (null, 404);

            await _repository.DeleteAsync(id);
            return (existe, 200);
        }

        // Validaciones HU SRV15.
        // 400 = falta un dato requerido. 422 = el dato viene pero con formato invalido.
        private static int Validar(ParametroRequest r)
        {
            if (string.IsNullOrWhiteSpace(r.Id) || string.IsNullOrWhiteSpace(r.Valor))
                return 400;
            if (r.Id.Length > 10)
                return 422;
            if (!Regex.IsMatch(r.Id, @"^[A-Z]+$"))
                return 422;
            if (r.Valor.Length > 500)
                return 422;
            return 0;
        }
    }
}
