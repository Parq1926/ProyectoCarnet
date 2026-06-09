using SRV3_Carreras;
using SRV3_Carreras.Repository;
using SRV3_Carreras.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<ICarreraService, CarreraService>();

var app = builder.Build();

app.MapCarreraEndpoints();

app.Run();