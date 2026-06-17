using Dapper;
using BitacoraSRV9.Entities;

namespace BitacoraSRV9.Repository;

public class BitacoraRepository
{
    private readonly IDbConnectionFactory _db;

    public BitacoraRepository(IDbConnectionFactory db)
    {
        _db = db;
    }

    public async Task<int> GuardarAsync(Bitacora bitacora)
    {
        using var conn = _db.CreateConnection();

        return await conn.ExecuteAsync(
        @"
        INSERT INTO BITACORA
        (
            USUARIO,
            ACCION,
            FECHA
        )
        VALUES
        (
            @Usuario,
            @Accion,
            @Fecha
        )",
        bitacora);
    }


    public async Task<IEnumerable<Bitacora>> ObtenerTodosAsync()
    {
        using var conn = _db.CreateConnection();

        return await conn.QueryAsync<Bitacora>(
        @"
        SELECT
            ID AS Id,
            USUARIO AS Usuario,
            ACCION AS Accion,
            FECHA AS Fecha
        FROM BITACORA
        ORDER BY FECHA DESC");
    }
}