using BitacoraSRV9;
using BitacoraSRV9.Auth;
using BitacoraSRV9.Repository;
using BitacoraSRV9.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ITokenValidator, TokenValidator>();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<BitacoraRepository>();

builder.Services.AddScoped<IBitacoraService, BitacoraService>();

var app = builder.Build();

app.MapBitacoraEndpoints();

app.Run();