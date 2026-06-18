using SRV4_Areas;
using SRV4_Areas.Auth;
using SRV4_Areas.Repository;
using SRV4_Areas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IAreaService, AreaService>();

builder.Services.AddHttpClient();

builder.Services.AddHttpClient<IBitacoraClient, BitacoraClient>();

builder.Services.AddScoped<ITokenValidator, TokenValidator>();

var app = builder.Build();

app.MapAreaEndpoints();

app.Run();