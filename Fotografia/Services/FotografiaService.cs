using SRV13_Fotografia.Entities;
using SRV13_Fotografia.Repository;

namespace SRV13_Fotografia.Services
{
    public class FotografiaService : IFotografiaService
    {
        private readonly FotografiaRepository _repository;

        // HU SRV13: idealmente la imagen no debe superar 1 MB.
        private const int MAX_BYTES = 1024 * 1024;

        public FotografiaService(FotografiaRepository repository) { _repository = repository; }

        public async Task<FotografiaUsuario?> ObtenerFotografiaAsync(string identificacion) =>
            await _repository.ObtenerPorIdentificacionAsync(identificacion);

        public async Task<(FotografiaUsuario? data, int status)> ActualizarFotografiaAsync(string identificacion, string fotografiaBase64)
        {
            // El usuario debe existir para poder asignarle una foto.
            if (!await _repository.UsuarioExisteAsync(identificacion))
                return (null, 404);

            // Debe ser una cadena Base64 valida.
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(fotografiaBase64);
            }
            catch (FormatException)
            {
                return (null, 422);
            }

            // Tamaño maximo 1 MB (HU SRV13).
            if (bytes.Length > MAX_BYTES)
                return (null, 422);

            var rows = await _repository.ActualizarFotografiaAsync(identificacion, fotografiaBase64);
            if (rows <= 0) return (null, 500);

            var saved = await _repository.ObtenerPorIdentificacionAsync(identificacion);
            return (saved, 200);
        }

        public async Task<FotografiaUsuario?> EliminarFotografiaAsync(string identificacion)
        {
            var actual = await _repository.ObtenerPorIdentificacionAsync(identificacion);
            if (actual is null) return null;
            await _repository.EliminarFotografiaAsync(identificacion);
            return actual;
        }
    }
}
