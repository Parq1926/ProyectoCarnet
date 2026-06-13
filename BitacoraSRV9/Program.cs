using BitacoraSRV9.Data;
using BitacoraSRV9.Entities;
using BitacoraSRV9.Repository;
using BitacoraSRV9.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BitacoraDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<BitacoraRepository>();
builder.Services.AddScoped<BitacoraService>();

var app = builder.Build();

app.MapPost("/bitacora",
async (
    Bitacora bitacora,
    BitacoraService service) =>
{
    await service.Registrar(
        bitacora.Usuario,
        bitacora.Accion);

    return Results.Ok("Movimiento registrado");
});

app.MapGet("/bitacora",
(BitacoraService service) =>
{
    return Results.Ok(
        service.ObtenerTodos());
});

Console.WriteLine(
    builder.Configuration.GetConnectionString("DefaultConnection"));

app.Run();