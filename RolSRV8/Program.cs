using Microsoft.EntityFrameworkCore;
using RolSRV8.Data;
using RolSRV8.Entities;
using RolSRV8.Repository;
using RolSRV8.Services;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RolDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RolRepository>();
builder.Services.AddScoped<RolService>();

var app = builder.Build();

app.MapPost("/rol",
async (
    Rol rol,
    RolService service) =>
{
    await service.Crear(rol);

    var cliente = new HttpClient();

    await cliente.PostAsJsonAsync(
        "http://localhost:5209/bitacora",
        new
        {
            Usuario = "Administrador del Sistema",
            Accion = $"Creó el rol {rol.Nombre}"
        });

    return Results.Ok("Rol creado");
});

app.MapGet("/rol",
(RolService service) =>
{
    return Results.Ok(
        service.ObtenerTodos());
});

app.MapGet("/rol/{id}",
(int id,
 RolService service) =>
{
    var rol = service.ObtenerPorId(id);

    if (rol == null)
        return Results.NotFound("Rol no encontrado");

    return Results.Ok(rol);
});

app.MapPut("/rol/{id}",
async (
    int id,
    Rol rol,
    RolService service) =>
{
    await service.Actualizar(id, rol);

    var cliente = new HttpClient();

    await cliente.PostAsJsonAsync(
        "http://localhost:5209/bitacora",
        new
        {
            Usuario = "Administrador del Sistema",
            Accion = $"Modificó el rol {rol.Nombre}"
        });

    return Results.Ok("Rol actualizado");
});

app.MapDelete("/rol/{id}",
async (
    int id,
    RolService service) =>
{
    var rol = service.ObtenerPorId(id);

    if (rol == null)
        return Results.NotFound("Rol no encontrado");

    await service.Eliminar(id);

    var cliente = new HttpClient();

    await cliente.PostAsJsonAsync(
        "http://localhost:5209/bitacora",
        new
        {
            Usuario = "Administrador del Sistema",
            Accion = $"Eliminó el rol {rol.Nombre}"
        });

    return Results.Ok("Rol eliminado");
});

app.Run();