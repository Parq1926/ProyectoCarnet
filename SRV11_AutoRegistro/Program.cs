using SRV11_Autoregistro;
using SRV11_AutoRegistro.Services;
using SRV11_AutoRegistro.Repository;
using SRV2_Instituciones.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<UsuarioRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<BitacoraService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapAutoregistroEndpoints();

app.Run();