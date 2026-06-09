using SRV2_Instituciones.Entities;
using SRV2_Instituciones.Repository;
using System.Text.RegularExpressions;

namespace SRV2_Instituciones.Services;

public class InstitucionService : IInstitucionService
{
    private readonly IInstitucionRepository _repository;

    public InstitucionService(IInstitucionRepository repository)
    {
        _repository = repository;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhone(string telefono)
    {
        return !string.IsNullOrWhiteSpace(telefono) && Regex.IsMatch(telefono, @"^\d+$");
    }

    private bool IsValidDomain(string dominios)
    {
        if (string.IsNullOrWhiteSpace(dominios)) return false;
        var domains = dominios.Split(',');
        foreach (var domain in domains)
        {
            if (string.IsNullOrWhiteSpace(domain)) return false;
            if (!Regex.IsMatch(domain.Trim(), @"^[a-zA-Z0-9][a-zA-Z0-9.-]*\.[a-zA-Z]{2,}$"))
                return false;
        }
        return true;
    }

    public async Task<IEnumerable<Institucion>> GetAll()
    {
        return await _repository.GetAll();
    }

    public async Task<Institucion?> GetById(int id)
    {
        if (id <= 0) return null;
        return await _repository.GetById(id);
    }

    public async Task<(bool success, string message, int? id)> Create(CreateInstitucionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
            return (false, "El nombre es requerido", null);

        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            return (false, "El email no es valido", null);

        if (string.IsNullOrWhiteSpace(request.Telefono) || !IsValidPhone(request.Telefono))
            return (false, "El telefono solo debe contener numeros", null);

        if (string.IsNullOrWhiteSpace(request.Dominios) || !IsValidDomain(request.Dominios))
            return (false, "Los dominios no son validos. Ejemplo: cuc.ac.cr,www.cuc.ac.cr", null);

        if (await _repository.ExistsByNombre(request.Nombre))
            return (false, $"Ya existe una institucion con el nombre '{request.Nombre}'", null);

        var institucion = new Institucion
        {
            Nombre = request.Nombre.Trim(),
            Email = request.Email.Trim(),
            Telefono = request.Telefono.Trim(),
            Dominios = request.Dominios.Trim()
        };

        var id = await _repository.Create(institucion);
        return (true, "Institucion creada exitosamente", id);
    }

    public async Task<(bool success, string message)> Update(UpdateInstitucionRequest request)
    {
        if (request.ID <= 0)
            return (false, "ID invalido");

        var existing = await _repository.GetById(request.ID);
        if (existing == null)
            return (false, "Institucion no encontrada");

        if (string.IsNullOrWhiteSpace(request.Nombre))
            return (false, "El nombre es requerido");

        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            return (false, "El email no es valido");

        if (string.IsNullOrWhiteSpace(request.Telefono) || !IsValidPhone(request.Telefono))
            return (false, "El telefono solo debe contener numeros");

        if (string.IsNullOrWhiteSpace(request.Dominios) || !IsValidDomain(request.Dominios))
            return (false, "Los dominios no son validos");

        if (await _repository.ExistsByNombre(request.Nombre, request.ID))
            return (false, $"Ya existe otra institucion con el nombre '{request.Nombre}'");

        existing.Nombre = request.Nombre.Trim();
        existing.Email = request.Email.Trim();
        existing.Telefono = request.Telefono.Trim();
        existing.Dominios = request.Dominios.Trim();

        var updated = await _repository.Update(existing);
        return updated ? (true, "Institucion actualizada exitosamente") : (false, "No se pudo actualizar la institucion");
    }

    public async Task<(bool success, string message)> Delete(int id)
    {
        if (id <= 0)
            return (false, "ID invalido");

        var existing = await _repository.GetById(id);
        if (existing == null)
            return (false, "Institucion no encontrada");

        var deleted = await _repository.Delete(id);
        return deleted ? (true, "Institucion eliminada exitosamente") : (false, "No se pudo eliminar la institucion");
    }
}