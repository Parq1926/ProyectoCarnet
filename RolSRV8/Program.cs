using RolSRV8;
using RolSRV8.Auth;
using RolSRV8.Repository;
using RolSRV8.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<ITokenValidator, TokenValidator>();

builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<RolRepository>();

builder.Services.AddScoped<IRolService, RolService>();

builder.Services.AddHttpClient<IBitacoraClient,BitacoraClient>();
var app = builder.Build();

app.MapRolEndpoints();

app.Run();