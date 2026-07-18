// UsuariosSRV4/Services/UsuarioService.cs
using Microsoft.EntityFrameworkCore;
using UsuariosSRV4.Data;
using UsuariosSRV4.DTOs;
using UsuariosSRV4.Entities;
using UsuariosSRV4.Helpers;

namespace UsuariosSRV4.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _db;

        public UsuarioService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            return await _db.Usuarios
                .Where(u => u.Activo)
                .Include(u => u.Telefonos)
                .OrderBy(u => u.NombreCompleto)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    NumeroIdentificacion = u.NumeroIdentificacion,
                    NombreCompleto = u.NombreCompleto,
                    TipoUsuario = GetTipoUsuarioNombre(u.TipoUsuarioId),
                    TipoIdentificacion = GetTipoIdentificacionNombre(u.TipoIdentificacionId),
                    Activo = u.Activo,
                    Bloqueado = u.Bloqueado,
                    IntentosFallidos = u.IntentosFallidos,
                    FechaCreacion = u.FechaCreacion,
                    Telefonos = u.Telefonos != null ? u.Telefonos.Select(t => t.Numero).ToList() : new List<string>()
                })
                .ToListAsync();
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var user = await _db.Usuarios
                .Include(u => u.Telefonos)
                .FirstOrDefaultAsync(u => u.Id == id && u.Activo);

            if (user == null) return null;

            return new UsuarioDto
            {
                Id = user.Id,
                Email = user.Email,
                NumeroIdentificacion = user.NumeroIdentificacion,
                NombreCompleto = user.NombreCompleto,
                TipoUsuario = GetTipoUsuarioNombre(user.TipoUsuarioId),
                TipoIdentificacion = GetTipoIdentificacionNombre(user.TipoIdentificacionId),
                Activo = user.Activo,
                Bloqueado = user.Bloqueado,
                IntentosFallidos = user.IntentosFallidos,
                FechaCreacion = user.FechaCreacion,
                Telefonos = user.Telefonos != null ? user.Telefonos.Select(t => t.Numero).ToList() : new List<string>()
            };
        }

        public async Task<bool> DesbloquearUsuarioAsync(int id)
        {
            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return false;

            user.Bloqueado = false;
            user.Activo = true;
            user.IntentosFallidos = 0;
            user.FechaModificacion = DateTime.Now;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<UsuarioDto?> ValidarCredencialesAsync(string email, string password, string tipo)
        {
            Console.WriteLine($"=== VALIDANDO CREDENCIALES ===");
            Console.WriteLine($"Email: {email}");

            var user = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                Console.WriteLine("❌ Usuario no encontrado");
                return null;
            }

            // VERIFICAR SI EL USUARIO ESTÁ BLOQUEADO
            if (user.Bloqueado)
            {
                Console.WriteLine("❌ Usuario bloqueado permanentemente");
                return null;
            }

            // Verificar si el usuario está activo
            if (!user.Activo)
            {
                Console.WriteLine("❌ Usuario inactivo");
                return null;
            }

            // ✅ VERIFICAR CONTRASEÑA CON HASH
            var passwordHash = PasswordHelper.HashPassword(password);
            Console.WriteLine($"Hash ingresado: {passwordHash}");
            Console.WriteLine($"Hash en BD: {user.Contrasena}");

            if (user.Contrasena != passwordHash)
            {
                Console.WriteLine("❌ Contraseña incorrecta");

                // INCREMENTAR INTENTOS FALLIDOS
                user.IntentosFallidos++;

                // SI LLEGA A 3 INTENTOS, BLOQUEAR PERMANENTEMENTE
                if (user.IntentosFallidos >= 3)
                {
                    user.Bloqueado = true;
                    user.Activo = false;
                    Console.WriteLine($"❌ Usuario bloqueado permanentemente después de {user.IntentosFallidos} intentos fallidos");
                }

                await _db.SaveChangesAsync();
                return null;
            }

            // LOGIN EXITOSO - RESETEAR INTENTOS FALLIDOS
            if (user.IntentosFallidos > 0)
            {
                user.IntentosFallidos = 0;
                await _db.SaveChangesAsync();
            }

            // Verificar tipo
            var tipoUsuario = GetTipoUsuarioNombre(user.TipoUsuarioId);
            if (tipoUsuario != tipo)
            {
                Console.WriteLine($"❌ Tipo incorrecto: {tipoUsuario} != {tipo}");
                return null;
            }

            Console.WriteLine("✅ Login exitoso");

            return new UsuarioDto
            {
                Id = user.Id,
                Email = user.Email,
                NombreCompleto = user.NombreCompleto,
                TipoUsuario = tipoUsuario,
                Activo = user.Activo,
                Bloqueado = user.Bloqueado,
                IntentosFallidos = user.IntentosFallidos
            };
        }

        public async Task<IEnumerable<UsuarioDto>> GetByFilterAsync(FiltroUsuarioDto filtro)
        {
            var query = _db.Usuarios
                .Include(u => u.Telefonos)
                .Where(u => u.Activo);

            if (!string.IsNullOrEmpty(filtro.Identificacion))
            {
                query = query.Where(u => u.NumeroIdentificacion.Contains(filtro.Identificacion));
            }

            if (!string.IsNullOrEmpty(filtro.Nombre))
            {
                query = query.Where(u => u.NombreCompleto.Contains(filtro.Nombre));
            }

            // ✅ CORREGIDO: Usar el campo directamente en lugar del método
            if (!string.IsNullOrEmpty(filtro.TipoUsuario))
            {
                // Obtener el ID del tipo según el nombre
                int tipoId = filtro.TipoUsuario switch
                {
                    "Estudiante" => 1,
                    "Funcionario" => 2,
                    "Administrador" => 3,
                    _ => 0
                };

                if (tipoId > 0)
                {
                    query = query.Where(u => u.TipoUsuarioId == tipoId);
                }
            }

            return await query
                .OrderBy(u => u.NombreCompleto)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    NumeroIdentificacion = u.NumeroIdentificacion,
                    NombreCompleto = u.NombreCompleto,
                    TipoUsuario = GetTipoUsuarioNombre(u.TipoUsuarioId),
                    TipoIdentificacion = GetTipoIdentificacionNombre(u.TipoIdentificacionId),
                    Activo = u.Activo,
                    Bloqueado = u.Bloqueado,
                    IntentosFallidos = u.IntentosFallidos,
                    FechaCreacion = u.FechaCreacion,
                    Telefonos = u.Telefonos != null ? u.Telefonos.Select(t => t.Numero).ToList() : new List<string>()
                })
                .ToListAsync();
        }

        public async Task<(bool ok, string error, UsuarioDto? data)> CreateAsync(CrearUsuarioDto dto)
        {
            try
            {
                if (await ExistsByEmailAsync(dto.Email))
                {
                    return (false, $"Ya existe un usuario con el email '{dto.Email}'", null);
                }

                if (await ExistsByIdentificacionAsync(dto.NumeroIdentificacion))
                {
                    return (false, $"Ya existe un usuario con la identificación '{dto.NumeroIdentificacion}'", null);
                }

                var validacion = await ValidarDominioYTipoAsync(dto.Email, dto.TipoUsuarioId);
                if (!validacion)
                {
                    return (false, "El dominio del email no coincide con el tipo de usuario", null);
                }

                var entity = new Usuario
                {
                    Email = dto.Email.Trim().ToLower(),
                    Contrasena = dto.Contrasena,  // ✅ Texto plano
                    TipoIdentificacionId = dto.TipoIdentificacionId,
                    NumeroIdentificacion = dto.NumeroIdentificacion.Trim(),
                    NombreCompleto = dto.NombreCompleto.Trim(),
                    TipoUsuarioId = dto.TipoUsuarioId,
                    Activo = true,
                    Bloqueado = false,
                    IntentosFallidos = 0,
                    FechaCreacion = DateTime.Now
                };

                _db.Usuarios.Add(entity);
                await _db.SaveChangesAsync();

                if (dto.Telefonos != null && dto.Telefonos.Any())
                {
                    foreach (var telefono in dto.Telefonos)
                    {
                        if (!string.IsNullOrEmpty(telefono))
                        {
                            _db.Telefonos.Add(new Telefono
                            {
                                UsuarioId = entity.Id,
                                Numero = telefono.Trim()
                            });
                        }
                    }
                    await _db.SaveChangesAsync();
                }

                var result = await GetByIdAsync(entity.Id);
                return (true, string.Empty, result);
            }
            catch (Exception ex)
            {
                return (false, $"Error al crear usuario: {ex.Message}", null);
            }
        }

        public async Task<(bool ok, string error, UsuarioDto? data)> UpdateAsync(int id, ActualizarUsuarioDto dto)
        {
            try
            {
                var entity = await _db.Usuarios
                    .Include(u => u.Telefonos)
                    .FirstOrDefaultAsync(u => u.Id == id && u.Activo);

                if (entity == null)
                {
                    return (false, $"Usuario con ID {id} no encontrado", null);
                }

                if (await ExistsByEmailAsync(dto.Email, id))
                {
                    return (false, $"Ya existe un usuario con el email '{dto.Email}'", null);
                }

                if (await ExistsByIdentificacionAsync(dto.NumeroIdentificacion, id))
                {
                    return (false, $"Ya existe un usuario con la identificación '{dto.NumeroIdentificacion}'", null);
                }

                var validacion = await ValidarDominioYTipoAsync(dto.Email, dto.TipoUsuarioId);
                if (!validacion)
                {
                    return (false, "El dominio del email no coincide con el tipo de usuario", null);
                }

                entity.Email = dto.Email.Trim().ToLower();
                entity.TipoIdentificacionId = dto.TipoIdentificacionId;
                entity.NumeroIdentificacion = dto.NumeroIdentificacion.Trim();
                entity.NombreCompleto = dto.NombreCompleto.Trim();
                entity.TipoUsuarioId = dto.TipoUsuarioId;
                entity.Activo = dto.Activo;
                entity.FechaModificacion = DateTime.Now;

                // ✅ Actualizar contraseña (texto plano)
                if (!string.IsNullOrEmpty(dto.Contrasena))
                {
                    entity.Contrasena = dto.Contrasena;
                }

                if (entity.Telefonos != null)
                {
                    _db.Telefonos.RemoveRange(entity.Telefonos);
                }

                if (dto.Telefonos != null && dto.Telefonos.Any())
                {
                    foreach (var telefono in dto.Telefonos)
                    {
                        if (!string.IsNullOrEmpty(telefono))
                        {
                            _db.Telefonos.Add(new Telefono
                            {
                                UsuarioId = entity.Id,
                                Numero = telefono.Trim()
                            });
                        }
                    }
                }

                await _db.SaveChangesAsync();

                var result = await GetByIdAsync(id);
                return (true, string.Empty, result);
            }
            catch (Exception ex)
            {
                return (false, $"Error al actualizar usuario: {ex.Message}", null);
            }
        }

        public async Task<(bool ok, string error)> DeleteAsync(int id)
        {
            try
            {
                var entity = await _db.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == id && u.Activo);

                if (entity == null)
                {
                    return (false, $"Usuario con ID {id} no encontrado");
                }

                entity.Activo = false;
                entity.FechaModificacion = DateTime.Now;

                await _db.SaveChangesAsync();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar usuario: {ex.Message}");
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
        {
            var query = _db.Usuarios.Where(u => u.Email == email.Trim().ToLower() && u.Activo);
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByIdentificacionAsync(string identificacion, int? excludeId = null)
        {
            var query = _db.Usuarios.Where(u => u.NumeroIdentificacion == identificacion.Trim() && u.Activo);
            if (excludeId.HasValue)
            {
                query = query.Where(u => u.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> ValidarDominioYTipoAsync(string email, int tipoUsuarioId)
        {
            var domain = email.Split('@').Last().ToLower();

            if (tipoUsuarioId == 1) // Estudiante
            {
                return domain == "cuc.cr";
            }
            else if (tipoUsuarioId == 2 || tipoUsuarioId == 3) // Funcionario o Administrador
            {
                return domain == "cuc.ac.cr";
            }

            return false;
        }

        private static string GetTipoUsuarioNombre(int tipoUsuarioId)
        {
            return tipoUsuarioId switch
            {
                1 => "Estudiante",
                2 => "Funcionario",
                3 => "Administrador",
                _ => "Desconocido"
            };
        }

        private static string GetTipoIdentificacionNombre(int tipoIdentificacionId)
        {
            return tipoIdentificacionId switch
            {
                1 => "Cédula",
                2 => "Pasaporte",
                3 => "DIMEX",
                4 => "NITE",
                _ => "Desconocido"
            };
        }
    }
}