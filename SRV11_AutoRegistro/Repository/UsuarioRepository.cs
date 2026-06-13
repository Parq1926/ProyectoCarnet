using Dapper;
using SRV11_AutoRegistro.Entities;
using SRV2_Instituciones.Repository;

namespace SRV11_AutoRegistro.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> Create(Usuario usuario)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        INSERT INTO Usuario
        (
            Email,
            TipoIdentificacionID,
            Identificacion,
            NombreCompleto,
            PasswordHash,
            TipoUsuarioID,
            RolID,
            Confirmado,
            TokenConfirmacion,
            FechaExpiracion,
            Activo,
            FechaCreacion
        )
        VALUES
        (
            @Email,
            @TipoIdentificacionID,
            @Identificacion,
            @NombreCompleto,
            @PasswordHash,
            @TipoUsuarioID,
            @RolID,
            @Confirmado,
            @TokenConfirmacion,
            @FechaExpiracion,
            1,
            GETDATE()
        );

        SELECT CAST(SCOPE_IDENTITY() as int)";

        return await connection.QuerySingleAsync<int>(sql, usuario);
    }

    public async Task<Usuario?> GetByEmail(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        SELECT *
        FROM Usuario
        WHERE Email = @Email
        AND Activo = 1";

        return await connection.QueryFirstOrDefaultAsync<Usuario>(
            sql,
            new { Email = email });
    }

    public async Task<Usuario?> GetByToken(string token)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        SELECT *
        FROM Usuario
        WHERE TokenConfirmacion = @Token
        AND Activo = 1";

        return await connection.QueryFirstOrDefaultAsync<Usuario>(
            sql,
            new { Token = token });
    }

    public async Task<bool> ConfirmarCuenta(string token)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        UPDATE Usuario
        SET Confirmado = 1,
            TokenConfirmacion = NULL,
            FechaExpiracion = NULL,
            FechaModificacion = GETDATE()
        WHERE TokenConfirmacion = @Token";

        var rows = await connection.ExecuteAsync(
            sql,
            new { Token = token });

        return rows > 0;
    }


    public async Task<bool> ExistsByEmail(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = @"
        SELECT COUNT(1)
        FROM Usuario
        WHERE Email = @Email
        AND Activo = 1";

        var count = await connection.ExecuteScalarAsync<int>(
            sql,
            new { Email = email });

        return count > 0;
    }
}