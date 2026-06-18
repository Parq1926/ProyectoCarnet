using Dapper;
using SRV13_Fotografia.Entities;

namespace SRV13_Fotografia.Repository
{
    public class FotografiaRepository
    {
        private readonly IDbConnectionFactory _db;

        public FotografiaRepository(IDbConnectionFactory db) { _db = db; }

        // Valida que el usuario exista en la tabla USUARIOS.
        public async Task<bool> UsuarioExisteAsync(string identificacion)
        {
            using var conn = _db.CreateConnection();
            var id = await conn.QueryFirstOrDefaultAsync<string?>(
                "SELECT USUARIO_IDENTIFICACION FROM USUARIOS WHERE USUARIO_IDENTIFICACION = @identificacion",
                new { identificacion });
            return id is not null;
        }

        // Devuelve la foto del usuario (solo si tiene una).
        public async Task<FotografiaUsuario?> ObtenerPorIdentificacionAsync(string identificacion)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<FotografiaUsuario>(
                @"SELECT USUARIO_IDENTIFICACION AS UsuarioIdentificacion,
                         FOTOGRAFIA_BASE64 AS FotografiaBase64
                  FROM USUARIOS
                  WHERE USUARIO_IDENTIFICACION = @identificacion
                    AND FOTOGRAFIA_BASE64 IS NOT NULL",
                new { identificacion });
        }

        // Agrega o actualiza la foto del usuario (es solo la columna en USUARIOS).
        public async Task<int> ActualizarFotografiaAsync(string identificacion, string fotografiaBase64)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteAsync(
                @"UPDATE USUARIOS
                  SET FOTOGRAFIA_BASE64 = @fotografiaBase64
                  WHERE USUARIO_IDENTIFICACION = @identificacion",
                new { fotografiaBase64, identificacion });
        }

        // Elimina solo la foto (deja el usuario).
        public async Task<int> EliminarFotografiaAsync(string identificacion)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteAsync(
                @"UPDATE USUARIOS
                  SET FOTOGRAFIA_BASE64 = NULL
                  WHERE USUARIO_IDENTIFICACION = @identificacion",
                new { identificacion });
        }
    }
}
