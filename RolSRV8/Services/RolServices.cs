using System.Text.RegularExpressions;
using RolSRV8.Entities;
using RolSRV8.Repository;

namespace RolSRV8.Services;

public class RolService
{
    private readonly RolRepository _repository;

    public RolService(RolRepository repository)
    {
        _repository = repository;
    }

    public async Task Crear(Rol rol)
    {
        if (string.IsNullOrWhiteSpace(rol.Nombre))
            throw new Exception("Nombre requerido");

        if (string.IsNullOrWhiteSpace(rol.Pantallas))
            throw new Exception("Pantallas requeridas");

        if (!Regex.IsMatch(
            rol.Nombre,
            @"^[a-zA-Z0-9\s]+$"))
        {
            throw new Exception(
                "Nombre inválido");
        }

        await _repository.Guardar(rol);
    }

    public List<Rol> ObtenerTodos()
    {
        return _repository.ObtenerTodos();
    }

    public Rol? ObtenerPorId(int id)
    {
        return _repository.ObtenerPorId(id);
    }

    public async Task Actualizar(
        int id,
        Rol nuevoRol)
    {
        var rol = _repository.ObtenerPorId(id);

        if (rol == null)
            throw new Exception("Rol no encontrado");

        rol.Nombre = nuevoRol.Nombre;
        rol.Pantallas = nuevoRol.Pantallas;

        await _repository.Actualizar();
    }

    public async Task Eliminar(int id)
    {
        var rol = _repository.ObtenerPorId(id);

        if (rol == null)
            throw new Exception("Rol no encontrado");

        await _repository.Eliminar(rol);
    }
}