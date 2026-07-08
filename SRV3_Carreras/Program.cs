using SRV3_Carreras;
using SRV3_Carreras.Repository;
using SRV3_Carreras.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<ICarreraService, CarreraService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

builder.Services.AddHttpClient<IBitacoraService, BitacoraService>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.MapCarreraEndpoints();

app.Run();