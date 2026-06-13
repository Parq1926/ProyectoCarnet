using Microsoft.AspNetCore.Connections;
using SRV2_Instituciones;
using SRV2_Instituciones.Repository;
using SRV2_Instituciones.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IInstitucionRepository, InstitucionRepository>();
builder.Services.AddScoped<IInstitucionService, InstitucionService>();
builder.Services.AddScoped<IBitacoraService, BitacoraService>();

builder.Services.AddHttpClient<IBitacoraService, BitacoraService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapInstitucionEndpoints();

app.Run();